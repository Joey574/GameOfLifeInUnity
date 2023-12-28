using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameAttributes
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract bool CanCustomizeColor { get; }
}
