using UnityEngine;
using System.Collections;

public delegate void AnimationDelegate(float currentVal, float currentBufferVal);
public delegate void VoidDelegate();
public delegate string StringDelegate();
public delegate int IntDelegate();
public delegate float FloatDelegate();
public delegate bool BoolDelegate();

public delegate void IntParamDelegate(int param);
public delegate void FloatParamDelegate(float param);
public delegate void BoolParamDelegate(bool param);
public delegate void StringParamDelegate(string param);

//public delegate HitObj RaycastHitParamHitObjDelegate(ref RaycastHit hit);
public delegate bool IntParamBoolDelegate(int param);

public static class DelegateLibrary{
	
}
