using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModule {
  bool Activate();
  bool Desactivate();
  void OnActivation();
  void OnDesactivation();
}

public abstract class ModuleBase : MonoBehaviour, IModule {
  protected bool state = false;

  public bool Activate() {
    if (state) { return false; }
    state = true;
    OnActivation();
    return true;
  }

  public bool Desactivate() {
    if (!state) { return false; }
    state = false;
    OnDesactivation();
    return true;
  }

  public abstract void OnActivation();
  public abstract void OnDesactivation();
}
