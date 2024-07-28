/*********************************************
 * 
 * 脚本名：UISelect.cs
 * 创建时间：2023/01/06 13:57:39
 *********************************************/
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public class UISelectData
    {
        public string Title;
        public string Tmp_A;
        public string Tmp_B;
        public string Tmp_C;
        public string Tmp_D;
        public Action ACallback;
        public Action BCallback;
        public Action CCallback;
        public Action DCallback;
    }

    public partial class UISelect : GameUIBase
    {
        private UISelectData _data;

        public override void OnInit()
        {
            Btn_Mask.AddListener(OnClick_Btn_Mask);
            Btn_A.AddListener(OnClick_Btn_A);
            Btn_B.AddListener(OnClick_Btn_B);
            Btn_C.AddListener(OnClick_Btn_C);
            Btn_D.AddListener(OnClick_Btn_D);
        }

        public override void OnShow(params object[] args)
        {
            _data = args[0] as UISelectData;
            Btn_A.SetActive(!string.IsNullOrWhiteSpace(_data.Tmp_A));
            Btn_B.SetActive(!string.IsNullOrWhiteSpace(_data.Tmp_B));
            Btn_C.SetActive(!string.IsNullOrWhiteSpace(_data.Tmp_C));
            Btn_D.SetActive(!string.IsNullOrWhiteSpace(_data.Tmp_D));
            Tmp_Title.text = _data.Title;
            Tmp_A.text = _data.Tmp_A;
            Tmp_B.text = _data.Tmp_B;
            Tmp_C.text = _data.Tmp_C;
            Tmp_D.text = _data.Tmp_D;
        }

        private void OnClick_Btn_Mask()
        {
            CloseSelf();
        }

        private void OnClick_Btn_A()
        {
            _data.ACallback?.Invoke();
            CloseSelf();
        }

        private void OnClick_Btn_B()
        {
            _data.BCallback?.Invoke();
            CloseSelf();
        }

        private void OnClick_Btn_C()
        {
            _data.CCallback?.Invoke();
            CloseSelf();
        }

        private void OnClick_Btn_D()
        {
            _data.DCallback?.Invoke();
            CloseSelf();
        }

        protected override void OnBeforeDestroy() { }
    }
}
