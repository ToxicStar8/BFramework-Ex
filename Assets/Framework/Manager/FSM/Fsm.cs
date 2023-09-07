/*********************************************
 * BFramework
 * ״̬��
 * ����ʱ�䣺2023/04/06 15:00:23
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Framework
{
	/// <summary>
	/// ״̬��
	/// </summary>
	/// <typeparam name="T">FSMManager</typeparam>
	public class Fsm<T> : FsmBase where T : class
	{
		/// <summary>
		/// ״̬��ӵ����
		/// </summary>
		public T Owner { get; private set; }

		/// <summary>
		/// ״̬�ֵ�
		/// </summary>
		private Dictionary<sbyte, FsmState<T>> _stateDic;

		/// <summary>
		/// ��ǰ״̬
		/// </summary>
		private FsmState<T> _currState;

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="fsmId">״̬�����</param>
		/// <param name="owner">ӵ����</param>
		/// <param name="states">״̬����</param>
		public Fsm(int fsmId, T owner, FsmState<T>[] states) : base(fsmId)
		{
			_stateDic = new Dictionary<sbyte, FsmState<T>>();
			Owner = owner;

			//��״̬�����ֵ�
			int len = states.Length;
			for (int i = 0; i < len; i++)
			{
				FsmState<T> state = states[i];
				state.CurrFsm = this;
				state.OnInit();
				_stateDic[(sbyte)i] = state;
			}

			//����Ĭ��״̬
			CurrStateType = -1;
		}

        public override void OnUpdate()
		{
			_currState?.OnUpdate();
		}

		/// <summary>
		/// ��ȡ״̬
		/// </summary>
		/// <param name="stateType">״̬Type</param>
		/// <returns>״̬</returns>
		public FsmState<T> GetState(sbyte stateType)
		{
			FsmState<T> state = null;
			_stateDic.TryGetValue(stateType, out state);
			return state;
		}

		/// <summary>
		/// �л�״̬
		/// </summary>
		/// <param name="newState"></param>
		public void ChangeState(sbyte newState)
		{
			if (CurrStateType == newState) return;

			//����ΪĬ��ʱ���Ѿ��뿪���� ���뿪�ڶ���
			if (CurrStateType != -1 && _currState != null)
			{
				_currState.OnLeave();
			}
			CurrStateType = newState;
			//-1����Ĭ��״̬
			if(newState != -1)
			{
				_currState = _stateDic[CurrStateType];
				//������״̬
				_currState.OnEnter();
			}
		}

		/// <summary>
		/// �ر�״̬��
		/// </summary>
		public override void OnDispose()
		{
			if (_currState != null)
			{
				_currState.OnLeave();
			}

			foreach (KeyValuePair<sbyte, FsmState<T>> state in _stateDic)
			{
				state.Value.OnDestroy();
			}
			_stateDic.Clear();
		}
	}
}