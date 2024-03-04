﻿#if UNITY_ANDROID
using System;
using UnityEngine;

namespace JustTrack {
    internal class IntPromise : AndroidJavaProxy {
        private const string CLASS = "Promise";

        internal IntPromise(Action<int?> pResolve, Action<string> pReject) : base($"{SDKAndroidAgent.PACKAGE}.{CLASS}") {
            m_OnResolve = pResolve;
            m_OnFailure = pReject;
        }

        Action<int?> m_OnResolve;
        Action<string> m_OnFailure;

        /**
        * Called after the operation was successful.
        *
        * @param pResponse The data the operation produced.
        */
        void resolve(int? pResponse) {
            m_OnResolve(pResponse);
        }

        void resolve(AndroidJavaObject pResponse) {
            using (pResponse) {
                if (pResponse == null) {
                    m_OnResolve(null);
                } else {
                    m_OnResolve(pResponse.Call<int>("intValue"));
                }
            }
        }

        /**
        * Called in case an error which can noy be handled occurs. If this method is called, {@link #resolve(Object)}
        * will not be called anymore.
        *
        * @param pException The error which occurred.
        */
        void reject(AndroidJavaObject pException) {
            using (pException) {
                m_OnFailure.Invoke(pException.Call<string>("toString"));
            }
        }
    }
}
#endif
