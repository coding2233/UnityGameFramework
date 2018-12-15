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
using System.Reflection;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace GameFramework.Taurus
{
    public sealed class HotFixManager : GameFrameworkModule,IUpdate,IFixedUpdate
    {
		#region 属性
#if ILRuntime
		/// <summary>
		/// ILRuntime的入口
		/// </summary>
		private AppDomain _appdomain;
#else
		/// <summary>
		/// 反射程序集
		/// </summary>
		private Assembly _assembly;
#endif
		//热更新的开头函数
		private object _hotFixEntity;

        //热更新里面的反射类型
        private List<Type> _hotFixTypes;
        /// <summary>
        /// 获取热更新的所有类型
        /// </summary>
        /// <returns></returns>
        public List<Type> GetHotFixTypes
        {
            get
            {
                if (_hotFixTypes == null || _hotFixTypes.Count == 0)
                {
                    _hotFixTypes = new List<Type>();
#if ILRuntime
					if (this._appdomain != null)
					{
						foreach (var item in _appdomain.LoadedTypes.Values)
						{
							_hotFixTypes.Add(item.ReflectionType);
						}
					}
#else
					if (_assembly != null)
					{
						_hotFixTypes = _assembly.GetTypes().ToList();
					}
#endif
				}
                return _hotFixTypes;
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
            
        }
		
        /// <summary>
        ///  加载
        /// </summary>
        /// <param name="dllDatas"></param>
        /// <param name="pdbDatas"></param>
        public void LoadHotfixAssembly(byte[] dllDatas,byte[] pdbDatas=null)
        {
#if ILRuntime
			_appdomain = new AppDomain();
			if (pdbDatas != null)
            {
                using (System.IO.MemoryStream fs = new MemoryStream(dllDatas))
                {
                    using (System.IO.MemoryStream p = new MemoryStream(pdbDatas))
                    {
                        _appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
                    }
                }
            }
            else
                using (System.IO.MemoryStream fs = new MemoryStream(dllDatas))
                {
                        _appdomain.LoadAssembly(fs, null, new Mono.Cecil.Pdb.PdbReaderProvider());
                }
			
            InitializeILRuntime();
#else
			_assembly = Assembly.Load(dllDatas, pdbDatas);
#endif
			//运行热更新的入口
			RunHotFixInstantiate();
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

			_hotFixEntity = null;

#if ILRuntime
			_appdomain = null;
#else
			_assembly = null;
#endif

		}

		//运行更新的实例
		private void RunHotFixInstantiate()
        {
#if ILRuntime
			_hotFixEntity = _appdomain?.Instantiate("HotFix.Taurus.HotFixMode");
#else
			_hotFixEntity = _assembly?.CreateInstance("HotFix.Taurus.HotFixMode");
#endif
			if (_hotFixEntity == null)
				throw new GamekException("热更新实例化失败:HotFix.Taurus.HotFixMode");
		}

		#region 内部函数

#if ILRuntime
		void InitializeILRuntime()
        {
            //注册CLR绑定
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(_appdomain);

            //跨域继承的基类
            _appdomain.RegisterCrossBindingAdaptor(new Google.Protobuf.IMessageAdaptor());
            _appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor());
            _appdomain.DelegateManager.RegisterFunctionDelegate<Google.Protobuf.IMessageAdaptor.Adaptor>();
            _appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();


            _appdomain.DelegateManager.RegisterMethodDelegate<System.UInt16, System.Byte[]>();

            //这里做一些ILRuntime的注册，HelloWorld示例暂时没有需要注册的
            _appdomain.DelegateManager.RegisterMethodDelegate<System.Object, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            _appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.EventHandler<ILRuntime.Runtime.Intepreter.ILTypeInstance>((sender, e) =>
                {
                    ((Action<System.Object, ILRuntime.Runtime.Intepreter.ILTypeInstance>)act)(sender, e);
                });
            });
           

        }
#endif

		#endregion

	}
}
