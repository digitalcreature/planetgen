using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public struct RaycastHitComparer : IComparer<RaycastHit> {

  public int Compare(RaycastHit a, RaycastHit b) {
    return (int) Mathf.Sign(a.distance - b.distance);
  }

}
