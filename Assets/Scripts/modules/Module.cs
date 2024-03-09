using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModule {
  bool Activate();
  void OnActivation();
}

public abstract class ModuleBase : MonoBehaviour, IModule {
  private bool state = false;

  public bool Activate() {
    if (state) { return false; }
    state = true;
    OnActivation();
    return true;
  }

  public abstract void OnActivation();
}
