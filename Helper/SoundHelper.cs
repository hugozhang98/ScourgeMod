using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;

namespace ScourgeMod.Helper
{
    public static class SoundHelper
    {
        /// <summary>
        /// 带淡入淡出的单次音效，无需保存槽位。
        /// 每次调用都会播放一次，应在进入动作或阶段时调用，不能每帧调用。
        /// canPlay 决定是否可以开始播放，以及是否提前淡出。
        /// 音效会在素材结束前自动淡出。
        /// 淡入淡出的时间单位为逻辑帧（60 帧/秒），设为 0 时立即完成。
        /// </summary>
        public static void PlayOnce(
            SoundStyle soundStyle,
            Func<Vector2> getPosition,
            Func<bool> canPlay,
            int fadeInFrameCount = 10,
            int fadeOutFrameCount = 15
        )
        {
            Play(soundStyle, getPosition, canPlay, false, fadeInFrameCount, fadeOutFrameCount);
        }

        /// <summary>
        /// 带淡入淡出的循环音效，可在 AI 中每帧调用。
        /// soundSlot 用于避免重复播放，返回值需要保存到原来的 soundSlot。
        /// canPlay 同时决定是否可以开始播放，以及何时开始淡出。
        /// </summary>
        public static SlotId PlayLooped(
            SlotId soundSlot,
            SoundStyle soundStyle,
            Func<Vector2> getPosition,
            Func<bool> canPlay,
            int fadeInFrameCount = 10,
            int fadeOutFrameCount = 15
        )
        {
            if (
                Main.dedServ
                || Main.gameMenu
                || !SoundEngine.IsAudioSupported
                || SoundEngine.TryGetActiveSound(soundSlot, out _)
            )
                return soundSlot;

            //先检查播放条件，避免淡出结束后再次启动
            if (!canPlay())
                return soundSlot;

            return Play(
                soundStyle,
                getPosition,
                canPlay,
                true,
                fadeInFrameCount,
                fadeOutFrameCount
            );
        }

        private static SlotId Play(
            SoundStyle soundStyle,
            Func<Vector2> getPosition,
            Func<bool> canPlay,
            bool isLooped,
            int fadeInFrameCount,
            int fadeOutFrameCount
        )
        {
            if (Main.dedServ || Main.gameMenu || !SoundEngine.IsAudioSupported || !canPlay())
                return SlotId.Invalid;

            float fade = 0f;
            bool fadingOut = false;
            bool isFirstUpdate = true;
            long previousTimestamp = 0;
            double playedDuration = 0d;
            double soundDuration = 0d;

            return SoundEngine.PlaySound(
                soundStyle with
                {
                    IsLooped = isLooped,
                },
                getPosition(),
                sound =>
                {
                    if (Main.gameMenu)
                        return false;

                    long currentTimestamp = Stopwatch.GetTimestamp();
                    double elapsedTime = isFirstUpdate
                        ? 0d
                        : (currentTimestamp - previousTimestamp) / (double)Stopwatch.Frequency;
                    previousTimestamp = currentTimestamp;

                    if (isFirstUpdate)
                    {
                        //使用本次实际选中的素材，兼容随机音效变体
                        if (!isLooped)
                        {
                            soundDuration = sound.Style.GetSoundEffect().Duration.TotalSeconds;
                        }

                        isFirstUpdate = false;
                    }

                    //暂停期间不推进音效进度和淡入淡出
                    if (sound.Sound.State == SoundState.Paused)
                        return true;

                    if (sound.Sound.State == SoundState.Stopped)
                        return false;

                    sound.Position = getPosition();

                    //一旦开始淡出，就不再恢复淡入
                    if (!canPlay())
                    {
                        fadingOut = true;
                    }

                    float elapsedFrames = (float)(elapsedTime * 60d);

                    if (fadingOut)
                    {
                        fade =
                            fadeOutFrameCount > 0
                                ? MathHelper.Clamp(fade - elapsedFrames / fadeOutFrameCount, 0f, 1f)
                                : 0f;
                    }
                    else
                    {
                        fade =
                            fadeInFrameCount > 0
                                ? MathHelper.Clamp(fade + elapsedFrames / fadeInFrameCount, 0f, 1f)
                                : 1f;
                    }

                    float volume = fade;

                    if (!isLooped)
                    {
                        //音高会改变播放速度，需要按实际速度计算剩余时间
                        double playbackRate = Math.Pow(2d, sound.Pitch);
                        playedDuration += elapsedTime * playbackRate;

                        if (playedDuration >= soundDuration)
                            return false;

                        if (fadeOutFrameCount > 0)
                        {
                            float remainingFrames = (float)(
                                (soundDuration - playedDuration) * 60d / playbackRate
                            );
                            float endFade = MathHelper.Clamp(
                                remainingFrames / fadeOutFrameCount,
                                0f,
                                1f
                            );
                            volume = Math.Min(volume, endFade);
                        }
                    }

                    sound.Volume = MathHelper.SmoothStep(0f, 1f, volume);

                    //等音量降到零，再停止
                    return !fadingOut || fade > 0f;
                }
            );
        }
    }
}
