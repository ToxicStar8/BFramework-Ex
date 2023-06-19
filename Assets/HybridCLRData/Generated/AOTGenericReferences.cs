public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ constraint implement type
	// }} 

	// {{ AOT generic type
	//Framework.TableCtrlBase`1<System.Object>
	// }}

	public void RefMethods()
	{
		// System.Object Framework.DataManager::TryInitData<System.Object>()
		// System.Object Framework.GameBase::GetTableCtrl<System.Object>()
		// System.Void Framework.GameBase::OpenUI<System.Object>(Framework.E_UILevel,System.Object[])
		// System.Void Framework.UIManager::OpenUI<System.Object>(Framework.E_UILevel,System.Object[])
		// System.Object[] System.Array::Empty<System.Object>()
		// System.Object UnityEngine.Component::GetComponent<System.Object>()
		// System.Object UnityEngine.GameObject::GetComponent<System.Object>()
	}
}