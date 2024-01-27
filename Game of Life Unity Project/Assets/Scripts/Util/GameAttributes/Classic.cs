using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classic : GameAttributes
{
    public override string Name => "Classic";

    public override string Description => "The classic game of life, in this version cells will be born if they have 3 neighbors, " +
        "stay the same if they have 2 neighbors, and in any other case, die.";

    public override bool CanCustomizeColor => true;
}
