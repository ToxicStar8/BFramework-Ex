/*********************************************
 * BFramework
 * 状态机管理器
 * 创建时间：2023/04/06 15:00:23
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Framework
{
	/// <summary>
	/// 状态机管理器
	/// </summary>
	public class FsmManager :  ManagerBase
	{
		/// <summary>
		/// 状态机字典
		/// </summary>
		private Dictionary<int, FsmBase> _fsmDic;
		private List<FsmBase> _updateFsms;
		private bool _isDisposed;

		/// <summary>
		/// 状态机的临时编号
		/// </summary>
		private int _tempFsmIndex = 0;

		public override void OnAwake()
		{
			_fsmDic = new Dictionary<int, FsmBase>();
			_updateFsms = new List<FsmBase>();
			_tempFsmIndex = 0;
			_isDisposed = false;
		}
		/// <summary>
		/// 创建状态机
		/// </summary>
		/// <typeparam name="T">拥有者类型</typeparam>
		/// <param name="owner">拥有者</param>
		/// <param name="states">状态数组</param>
		/// <returns></returns>
		public Fsm<T> CreateFsm<T>(T owner, FsmState<T>[] states) where T : class
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(nameof(FsmManager));
			}

			var fsm = new Fsm<T>(_tempFsmIndex, owner, states);
			_fsmDic[_tempFsmIndex] = fsm;
			_tempFsmIndex++;
			return fsm;
		}

		/// <summary>
		/// 销毁状态机
		/// </summary>
		/// <param name="fsmId">状态机编号</param>
		public void ReleaseFsm(int fsmId)
		{
			if (_fsmDic.TryGetValue(fsmId, out var fsm))
			{
				_fsmDic.Remove(fsmId);
				fsm.OnDispose();
			}
		}
		public override void OnUpdate()
		{
			if (_isDisposed)
			{
				return;
			}

			_updateFsms.Clear();
			_updateFsms.AddRange(_fsmDic.Values);

			foreach (var fsm in _updateFsms)
			{
				if (_fsmDic.ContainsKey(fsm.FsmId))
				{
					fsm.OnUpdate();
				}
			}
		}
		public override void OnDispose()
		{
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;
			_updateFsms.Clear();
			_updateFsms.AddRange(_fsmDic.Values);
			_fsmDic.Clear();

			foreach (var fsm in _updateFsms)
			{
				fsm.OnDispose();
			}

			_updateFsms.Clear();
		}
	}
}
