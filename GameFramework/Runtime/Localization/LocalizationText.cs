﻿//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #UGUI本地化文本# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年7月22日 13点53分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    [RequireComponent(typeof(Text))]
    public sealed class LocalizationText : MonoBehaviour
    {
        [SerializeField]
        private string _text;
        private Text _uiText;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                SetUIText(_text);
            }
        }

        private void Start()
        {
            SetUIText(_text);
        }

        private void SetUIText(string key)
        {
            if(_uiText == null)
            {
                _uiText = GetComponent<Text>();
            }
            _uiText.text = GameFrameworkMode.GetModule<LocalizationManager>().Get(key);
        }

    }
}
