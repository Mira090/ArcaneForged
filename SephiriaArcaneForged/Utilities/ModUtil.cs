using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SephiriaArcaneForged.Utilities
{
    public static class ModUtil
    {
        #region region Delay関数
        /// <summary>
        /// 1フレーム待った後に処理する
        /// </summary>
        /// <param name="script"></param>
        /// <param name="delay"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Coroutine Delay(this MonoBehaviour script, Action callback)
        {
            return script.StartCoroutine(DelayEnumerator(callback));
        }
        /// <summary>
        /// Delay秒待った後に処理する
        /// </summary>
        /// <param name="script"></param>
        /// <param name="delay"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Coroutine Delay(this MonoBehaviour script, float delay, Action callback)
        {
            return script.StartCoroutine(DelayEnumerator(delay, callback));
        }
        /// <summary>
        /// predicateを満たすまで待った後に処理する
        /// </summary>
        /// <param name="script"></param>
        /// <param name="predicate"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Coroutine Delay(this MonoBehaviour script, Func<bool> predicate, Action callback)
        {
            return script.StartCoroutine(DelayEnumerator(predicate, callback));
        }
        /// <summary>
        /// predicateを満たすまで待って、Delay秒待った後に処理する
        /// </summary>
        /// <param name="script"></param>
        /// <param name="predicate"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Coroutine Delay(this MonoBehaviour script, Func<bool> predicate, float delay, Action callback)
        {
            return script.StartCoroutine(DelayEnumerator(predicate, delay, callback));
        }
        /// <summary>
        /// コルーチンが終了するまで待った後に処理する
        /// </summary>
        /// <param name="script"></param>
        /// <param name="delay"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Coroutine Delay(this MonoBehaviour script, IEnumerator delay, Action callback)
        {
            return script.StartCoroutine(DelayEnumerator(delay, callback));
        }
        private static IEnumerator DelayEnumerator(Action callback)
        {
            yield return null;
            callback.Invoke();
        }
        private static IEnumerator DelayEnumerator(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }
        private static IEnumerator DelayEnumerator(Func<bool> predicate, Action callback)
        {
            yield return new WaitUntil(predicate);
            callback.Invoke();
        }
        private static IEnumerator DelayEnumerator(Func<bool> predicate, float delay, Action callback)
        {
            yield return new WaitUntil(predicate);
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }
        private static IEnumerator DelayEnumerator(IEnumerator delay, Action callback)
        {
            yield return delay;
            callback.Invoke();
        }
        #endregion

        #region 乱数関連
        public static bool Percent(this int percent)
        {
            return Random.Range(0, 100) < percent;
        }
        public static bool Percent(this float percent)
        {
            return Random.Range(0, 100f) <= percent;
        }
        public static bool Chance(this float probability)
        {
            return Random.Range(0, 1f) <= probability;
        }
        #endregion

        public static IEnumerable<Transform> GetChilds(this Transform transform)
        {
            for(int q = 0; q < transform.childCount; q++)
            {
                yield return transform.GetChild(q);
            }
        }
    }
}
