/*********************************************
 * BFramework
 * 状态机
 * 创建时间：2023/04/06 15:00:23
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Framework
{
	/// <summary>
	/// 状态机
	/// </summary>
	/// <typeparam name="T">FSMManager</typeparam>
	public class Fsm<T> : FsmBase where T : class
	{
		/// <summary>
		/// 状态机拥有者
		/// </summary>
		public T Owner { get; private set; }

		/// <summary>
		/// 状态字典
		/// </summary>
		private Dictionary<sbyte, FsmState<T>> _stateDic;

		/// <summary>
		/// 当前状态
		/// </summary>
		private FsmState<T> _currState;

		private bool _isDisposed;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="fsmId">状态机编号</param>
		/// <param name="owner">拥有者</param>
		/// <param name="states">状态数组</param>
		public Fsm(int fsmId, T owner, FsmState<T>[] states) : base(fsmId)
		{
			_stateDic = new Dictionary<sbyte, FsmState<T>>();
			Owner = owner;

			//把状态加入字典
			int len = states.Length;
			for (int i = 0; i < len; i++)
			{
				FsmState<T> state = states[i];
				state.CurrFsm = this;
				state.OnAwake();
				_stateDic[(sbyte)i] = state;
			}

			//设置默认状态
			CurrStateType = -1;
		}

        public override void OnUpdate()
		{
			if (_isDisposed)
			{
				return;
			}

			_currState?.OnUpdate();
		}
		/// <summary>
		/// 获取状态
		/// </summary>
		/// <param name="stateType">状态Type</param>
		/// <returns>状态</returns>
		public FsmState<T> GetState(sbyte stateType)
		{
			FsmState<T> state = null;
			_stateDic.TryGetValue(stateType, out state);
			return state;
		}

		/// <summary>
		/// 切换状态
		/// </summary>
		/// <param name="newState"></param>
		public void ChangeState(sbyte newState)
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(nameof(Fsm<T>));
			}

			if (CurrStateType == newState)
			{
				return;
			}

			FsmState<T> nextState = null;
			if (newState != -1 && !_stateDic.TryGetValue(newState, out nextState))
			{
				throw new ArgumentOutOfRangeException(nameof(newState), newState, "状态不存在。");
			}

			_currState?.OnLeave();
			_currState = null;
			CurrStateType = newState;

			if (nextState != null)
			{
				_currState = nextState;
				_currState.OnEnter();
			}
		}
		/// <summary>
		/// 关闭状态机
		/// </summary>
		public override void OnDispose()
		{
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;
			_currState?.OnLeave();
			_currState = null;
			CurrStateType = -1;

			foreach (KeyValuePair<sbyte, FsmState<T>> state in _stateDic)
			{
				state.Value.OnDestroy();
				state.Value.CurrFsm = null;
			}

			_stateDic.Clear();
			Owner = null;
		}
	}
}
