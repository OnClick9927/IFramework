/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.146
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;
namespace IFramework.Tweens
{
    [ScriptVersion(20)]
    [VersionUpdate(20, "增加数组tween")]
    public static partial class TweenEx
    {
        public static ITween SetRecyle(this ITween tween, bool rec)
        {
            if (tween.recyled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.autoRecyle = rec;
            return tween;
        }
        public static ITween OnCompelete(this ITween tween, Action onCompelete)
        {
            if (tween.recyled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.onCompelete += onCompelete;
            return tween;
        }
        public static ITween SetLoop(this ITween tween, int loop, LoopType loopType)
        {
            if (tween.recyled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.loop = loop;
            tween.loopType = loopType;
            return tween;
        }



        public static ITween SetCurve(this ITween tween, ValueCurve curve)
        {
            var converter = ValueCurveCoverter.Allocate<ValueCurveCoverter>(tween.env).Config(curve);
            return tween.SetConverter(converter);
        }
        public static ITween SetAnimationCurve(this ITween tween, AnimationCurve curve)
        {
            var converter = AnimationCurveCoverter.Allocate<AnimationCurveCoverter>(tween.env).Config(curve);
            return tween.SetConverter(converter);
        }
        public static ITween SetEase(this ITween tween, Ease ease)
        {
            var converter = EaseCoverter.Allocate<EaseCoverter>(tween.env).Config(ease);
            return tween.SetConverter(converter);
        }





        public static ITween SetUpdateType(this ITween tween, TweenUpdateType type)
        {
            TweenSingleton.updateType = type;
            return tween;
        }
        public static ITween SetDeltaTime(this ITween tween, float delta)
        {
            TweenValue.deltaTime = delta;
            return tween;
        }
        public static ITween SetDelta(this ITween tween, float delta)
        {
            TweenValue.delta = delta;
            return tween;
        }
        public static ITween SetTimeScale(this ITween tween, float speed)
        {
            TweenValue.timeScale = speed;
            return tween;
        }




        public static IArrayTween<T> AllocateArrayTween<T>(EnvironmentType env) where T : struct
        {
            if (env != EnvironmentType.Ev0)
                TweenSingleton.Initialized();

            return RecyclableObject.Allocate<ArrayTween<T>>(env);
        }
        public static ISingleTween<T> AllocateSingleTween<T>(EnvironmentType env) where T : struct
        {
            if (env != EnvironmentType.Ev0)
                TweenSingleton.Initialized();
            return RecyclableObject.Allocate<SingleTween<T>>(env);
        }


        public static ITween<T> DoGoto<T>(T start, T end, float duration, Func<T> getter, Action<T> setter, bool snap, EnvironmentType env= EnvironmentType.Extra0) where T : struct
        {
#if UNITY_EDITOR
            env = EnvironmentType.Ev0;
#endif
            var tween = AllocateSingleTween<T>(env);
            tween.Config(start, end, duration, getter, setter, snap);
            tween.Run();
            return tween;
        }
        public static ITween<T> DoGoto<T>(T[] array, float duration, Func<T> getter, Action<T> setter, bool snap, EnvironmentType env = EnvironmentType.Extra0) where T : struct
        {
#if UNITY_EDITOR
            env = EnvironmentType.Ev0;
#endif
            var tween = AllocateArrayTween<T>(env);
            tween.Config(array, duration, getter, setter, snap);
            tween.Run();
            return tween;
        }
    }
    public static partial class TweenEx
    {
        public static ITween<Vector3> DoMove(this Transform target, Vector3 end, float duration, bool snap = false)
        {
            return DoGoto(target.position, end, duration, () => { return target.position; },
                    (value) => {
                        target.position = value;
                    }, snap
                );
        }
        public static ITween<float> DoMoveX(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.position.x, end, duration, () => { return target.position.x; }, (value) => {
                target.position = new Vector3(value, target.position.y, target.position.z);
            }, snap);
        }
        public static ITween<float> DoMoveY(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.position.y, end, duration, () => { return target.position.y; },
                             (value) => {
                                 target.position = new Vector3(target.position.x, value, target.position.z);
                             }, snap);
        }
        public static ITween<float> DoMoveZ(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.position.z, end, duration, () => { return target.position.z; }, (value) => {
                target.position = new Vector3(target.position.x, target.position.y, value);
            }, snap);
        }
        public static ITween<Vector3> DoLocalMove(this Transform target, Vector3 end, float duration, bool snap = false)
        {
            return DoGoto(target.localPosition, end, duration, () => { return target.localPosition; }, (value) => {
                target.localPosition = value;
            }, snap);
        }
        public static ITween<float> DoLocalMoveX(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.localPosition.x, end, duration, () => { return target.localPosition.x; }, (value) => {
                target.localPosition = new Vector3(value, target.localPosition.y, target.localPosition.z);
            }, snap);
        }
        public static ITween<float> DoLocalMoveY(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.localPosition.y, end, duration, () => { return target.localPosition.y; }, (value) => {
                target.localPosition = new Vector3(target.localPosition.x, value, target.localPosition.z);
            }, snap);
        }
        public static ITween<float> DoLocalMoveZ(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.localPosition.z, end, duration, () => { return target.localPosition.z; }, (value) => {
                target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, value);
            }, snap);
        }


        public static ITween<Vector3> DoScale(this Transform target, Vector3 end, float duration, bool snap = false)
        {
            return DoGoto(target.localScale, end, duration, () => { return target.localScale; }, (value) => {
                target.localScale = value;
            }, snap);
        }
        public static ITween<float> DoScaleX(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.localScale.x, end, duration, () => { return target.localScale.x; }, (value) => {
                target.localScale = new Vector3(value, target.localScale.y, target.localScale.z);
            }, snap);
        }
        public static ITween<float> DoScaleY(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.localScale.y, end, duration, () => { return target.localScale.y; }, (value) => {
                target.localScale = new Vector3(target.localScale.x, value, target.localScale.z);
            }, snap);
        }
        public static ITween<float> DoScaleZ(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.localScale.z, end, duration, () => { return target.localScale.z; }, (value) => {
                target.localScale = new Vector3(target.localScale.x, target.localScale.y, value);
            }, snap);
        }


        public static ITween<Quaternion> DoRota(this Transform target, Quaternion end, float duration, bool snap = false)
        {
            return DoGoto(target.rotation, end, duration, () => { return target.rotation; }, (value) => {
                target.rotation = value;
            }, snap);
        }
        public static ITween<Vector3> DoRota(this Transform target, Vector3 end, float duration, bool snap = false)
        {
            return DoGoto(target.rotation.eulerAngles, end, duration, () => { return target.rotation.eulerAngles; }, (value) => {
                target.rotation = Quaternion.Euler(value);
            }, snap);
        }
        public static ITween<Quaternion> DoRotaFast(this Transform target, Vector3 end, float duration, bool snap = false)
        {
            return DoGoto(target.rotation, Quaternion.Euler(end), duration, () => { return target.rotation; }, (value) => {
                target.rotation = value;
            }, snap);
        }

        public static ITween<Quaternion> DoLocalRota(this Transform target, Quaternion end, float duration, bool snap = false)
        {
            return DoGoto(target.localRotation, end, duration, () => { return target.localRotation; }, (value) => {
                target.localRotation = value;
            }, snap);
        }
        public static ITween<Quaternion> DoLocalRota(this Transform target, Vector3 end, float duration, bool snap = false)
        {
            return DoGoto(target.localRotation, Quaternion.Euler(end), duration, () => { return target.localRotation; }, (value) => {
                target.localRotation = value;
            }, snap);
        }


        public static ITween<Color> DoColor(this Material target, Color end, float duration, bool snap = false)
        {
            return DoGoto(target.color, end, duration, () => { return target.color; }, (value) => {
                target.color = value;
            }, snap);
        }
        public static ITween<Color> DoColor(this Graphic target, Color end, float duration, bool snap = false)
        {
            return DoGoto(target.color, end, duration, () => { return target.color; }, (value) => {
                target.color = value;
            }, snap);
        }
        public static ITween<Color> DoColor(this Light target, Color end, float duration, bool snap = false)
        {
            return DoGoto(target.color, end, duration, () => { return target.color; }, (value) => {
                target.color = value;
            }, snap);
        }
        public static ITween<Color> DoColor(this Camera target, Color end, float duration, bool snap = false)
        {
            return DoGoto(target.backgroundColor, end, duration, () => { return target.backgroundColor; }, (value) => {
                target.backgroundColor = value;
            }, snap);
        }


        public static ITween<float> DoAlpha(this Material target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.color.a, end, duration, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this Graphic target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.color.a, end, duration, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this Light target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.color.a, end, duration, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this Camera target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.backgroundColor.a, end, duration, () => { return target.backgroundColor.a; }, (value) => {
                target.backgroundColor = new Color(target.backgroundColor.r, target.backgroundColor.g, target.backgroundColor.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this CanvasGroup target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.alpha, end, duration, () => { return target.alpha; }, (value) => {
                target.alpha = value;
            }, snap);
        }



        public static ITween<int> DoText(this Text target, int start, int end, float duration, bool snap = false)
        {
            return DoGoto(start, end, duration, () => {
                int value;
                if (int.TryParse(target.text, out value))
                    return value;
                return 0;
            }, (value) => {
                target.text = value.ToString();
            }, snap);
        }
        public static ITween<int> DoText(this Text target, string end, float duration)
        {
            return DoGoto(target.text.Length, end.Length, duration, () => { return target.text.Length; }, (value) => {
                target.text = end.Substring(0, value);
            }, false);
        }
        public static ITween<float> DoText(this Text target, float start, float end, float duration, bool snap = false)
        {
            return DoGoto(start, end, duration, () => {
                float value;
                if (float.TryParse(target.text, out value))
                    return value;
                return 0;
            }, (value) => {
                target.text = value.ToString();
            }, snap);
        }



        public static ITween<float> DoFillAmount(this Image target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.fillAmount, end, duration, () => { return target.fillAmount; }, (value) => {
                target.fillAmount = value;
            }, snap);
        }
        public static ITween<float> DoNormalizedPositionX(this ScrollRect target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.normalizedPosition.x, end, duration, () => { return target.normalizedPosition.x; }, (value) => {
                target.normalizedPosition = new Vector2(value, target.normalizedPosition.y);
            }, snap);
        }
        public static ITween<float> DoNormalizedPositionY(this ScrollRect target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.normalizedPosition.y, end, duration, () => { return target.normalizedPosition.y; }, (value) => {
                target.normalizedPosition = new Vector2(target.normalizedPosition.x, value);
            }, snap);
        }




        public static ITween<bool> DoActive(this GameObject target, bool end, float duration)
        {
            return DoGoto(target.activeSelf, end, duration, () => { return target.activeSelf; }, (value) => {
                target.SetActive(value);
            }, false);
        }
        public static ITween<bool> DoEnable(this Behaviour target, bool end, float duration)
        {
            return DoGoto(target.enabled, end, duration, () => { return target.enabled; }, (value) => {
                target.enabled = value;
            }, false);
        }
        public static ITween<bool> DoToggle(this Toggle target, bool end, float duration)
        {
            return DoGoto(target.isOn, end, duration, () => { return target.isOn; }, (value) => {
                target.isOn = value;
            }, false);
        }
    }
    public static partial class TweenEx
    {
        public static ITween<Vector3> DoMove(this Transform self, Vector3[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.position; }, (value) => { self.position = value; }, snap);
        }
        public static ITween<float> DoMoveX(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.position.x; }, (value) => { self.position = new Vector3(value, self.position.y, self.position.z); }, snap);
        }
        public static ITween<float> DoMoveY(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.position.y; }, (value) => { self.position = new Vector3(self.position.x, value, self.position.z); }, snap);
        }
        public static ITween<float> DoMoveZ(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.position.z; }, (value) => { self.position = new Vector3(self.position.x, self.position.y, value); }, snap);
        }


        public static ITween<Vector3> DoLocalMove(this Transform self, Vector3[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localPosition; }, (value) => { self.localPosition = value; }, snap);
        }
        public static ITween<float> DoLocalMoveX(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localPosition.x; }, (value) => { self.localPosition = new Vector3(value, self.localPosition.y, self.localPosition.z); }, snap);
        }
        public static ITween<float> DoLocalMoveY(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localPosition.y; }, (value) => { self.localPosition = new Vector3(self.localPosition.x, value, self.localPosition.z); }, snap);
        }
        public static ITween<float> DoLocalMoveZ(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localPosition.z; }, (value) => { self.localPosition = new Vector3(self.localPosition.x, self.localPosition.y, value); }, snap);
        }


        public static ITween<Vector3> DoScale(this Transform self, Vector3[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localScale; }, (value) => { self.localScale = value; }, snap);
        }
        public static ITween<float> DoScaleX(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localScale.x; }, (value) => { self.localScale = new Vector3(value, self.localScale.y, self.localScale.z); }, snap);
        }
        public static ITween<float> DoScaleY(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localScale.y; }, (value) => { self.localScale = new Vector3(self.localScale.x, value, self.localScale.z); }, snap);
        }
        public static ITween<float> DoScaleZ(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localScale.z; }, (value) => { self.localScale = new Vector3(self.localScale.x, self.localScale.y, value); }, snap);
        }

        public static ITween<Quaternion> DoRota(this Transform target, Quaternion[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.rotation; }, (value) => {
                target.rotation = value;
            }, snap);
        }
        public static ITween<Vector3> DoRota(this Transform target, Vector3[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.rotation.eulerAngles; }, (value) => {
                target.rotation = Quaternion.Euler(value);
            }, snap);
        }
        public static ITween<Quaternion> DoLocalRota(this Transform target, Quaternion[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.localRotation; }, (value) => {
                target.localRotation = value;
            }, snap);
        }


        public static ITween<Color> DoColor(this Material target, Color[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.color; }, (value) => {
                target.color = value;
            }, snap);
        }
        public static ITween<Color> DoColor(this Graphic target, Color[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.color; }, (value) => {
                target.color = value;
            }, snap);
        }
        public static ITween<Color> DoColor(this Light target, Color[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.color; }, (value) => {
                target.color = value;
            }, snap);
        }
        public static ITween<Color> DoColor(this Camera target, Color[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.backgroundColor; }, (value) => {
                target.backgroundColor = value;
            }, snap);
        }

        public static ITween<float> DoAlpha(this Material target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this Graphic target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this Light target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this Camera target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.backgroundColor.a; }, (value) => {
                target.backgroundColor = new Color(target.backgroundColor.r, target.backgroundColor.g, target.backgroundColor.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this CanvasGroup target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.alpha; }, (value) => {
                target.alpha = value;
            }, snap);
        }


        public static ITween<float> DoFillAmount(this Image target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.fillAmount; }, (value) => {
                target.fillAmount = value;
            }, snap);
        }
        public static ITween<float> DoNormalizedPositionX(this ScrollRect target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.normalizedPosition.x; }, (value) => {
                target.normalizedPosition = new Vector2(value, target.normalizedPosition.y);
            }, snap);
        }
        public static ITween<float> DoNormalizedPositionY(this ScrollRect target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.normalizedPosition.y; }, (value) => {
                target.normalizedPosition = new Vector2(target.normalizedPosition.x, value);
            }, snap);
        }

    }

}
