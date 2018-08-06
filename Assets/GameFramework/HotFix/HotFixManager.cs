//-----------------------------------------------------------------------
// <copyright file="HotFixManager.cs" company="北京塞傲时代科技有限公司">
//     Copyright (c) 塞傲时代. All rights reserved.
// </copyright>
// <describe> #热更新模块# </describe>
// <author> codingworks </author>
// <time> #2018年6月20日 17点17分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace GameFramework.Taurus
{
    public sealed class HotFixManager : GameFrameworkModule,IUpdate,IFixedUpdate
    {
        #region 属性
        /// <summary>
        /// ILRuntime的入口
        /// </summary>
        public AppDomain Appdomain { get; private set; }
        //热更新的开头函数
        private object _hotFixVrCoreEntity;

        //热更新里面的反射类型
        private List<Type> _hotFixReflectionTypes;
        /// <summary>
        /// 获取热更新的所有类型
        /// </summary>
        /// <returns></returns>
        public List<Type> GetHotFixTypes
        {
            get
            {
                if (_hotFixReflectionTypes == null || _hotFixReflectionTypes.Count == 0)
                {
                    _hotFixReflectionTypes = new List<Type>();
                    if (this.Appdomain == null)
                        return _hotFixReflectionTypes;

                    foreach (var item in Appdomain.LoadedTypes.Values)
                    {
                        _hotFixReflectionTypes.Add(item.ReflectionType);
                    }
                }
                return _hotFixReflectionTypes;
            }
        }

        #region 函数
        //渲染更新函数
        public Action Update;
        //固定帧更新函数
        public Action FixedUpdate;
       //结束函数
        public Action Close;
        #endregion

        #endregion

        public HotFixManager()
        {
            Appdomain = new AppDomain();
        }


        /// <summary>
        ///  加载
        /// </summary>
        /// <param name="dllDatas"></param>
        /// <param name="pdbDatas"></param>
        public void LoadHotfixAssembly(byte[] dllDatas,byte[] pdbDatas=null)
        {
            if (pdbDatas != null)
            {
                using (System.IO.MemoryStream fs = new MemoryStream(dllDatas))
                {
                    using (System.IO.MemoryStream p = new MemoryStream(pdbDatas))
                    {
                        Appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
                    }
                }
            }
            else
                using (System.IO.MemoryStream fs = new MemoryStream(dllDatas))
                {
                        Appdomain.LoadAssembly(fs, null, new Mono.Cecil.Pdb.PdbReaderProvider());
                }


            InitializeILRuntime();

            //运行热更新的入口
            RunHotFixVrCoreEntity();
        }

        void InitializeILRuntime()
        {
            //注册CLR绑定
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(Appdomain);

            //跨域继承的基类
            Appdomain.RegisterCrossBindingAdaptor(new Google.Protobuf.IMessageAdaptor());
            Appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor());
            Appdomain.DelegateManager.RegisterFunctionDelegate<Google.Protobuf.IMessageAdaptor.Adaptor>();
            Appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();


            Appdomain.DelegateManager.RegisterMethodDelegate<System.UInt16, System.Byte[]>();

            //这里做一些ILRuntime的注册，HelloWorld示例暂时没有需要注册的
            Appdomain.DelegateManager.RegisterMethodDelegate<System.Object, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            Appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.EventHandler<ILRuntime.Runtime.Intepreter.ILTypeInstance>((sender, e) =>
                {
                    ((Action<System.Object, ILRuntime.Runtime.Intepreter.ILTypeInstance>)act)(sender, e);
                });
            });
           

        }
        
        public void OnUpdate()
        {
            Update?.Invoke();
        }

        public void OnFixedUpdate()
        {
            FixedUpdate?.Invoke();
        }

        public override void OnClose()
        {
            Close?.Invoke();
            Appdomain = null;
        }

        //运行更新的实例
        private void RunHotFixVrCoreEntity()
        {
            _hotFixVrCoreEntity = Appdomain.Instantiate("HotFix.Taurus.HotFixMode");
        }

    }
}
