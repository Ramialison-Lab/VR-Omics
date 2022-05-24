﻿/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using Facebook.WitAi.Data.Traits;
using UnityEditor;

namespace Facebook.WitAi.Windows
{
    [CustomPropertyDrawer(typeof(WitTraitValue))]
    public class WitTraitValuePropertyDrawer : WitSimplePropertyDrawer
    {
        // Key = value
        protected override string GetKeyFieldName()
        {
            return "value";
        }
        // Value = id
        protected override string GetValueFieldName()
        {
            return "id";
        }
    }
}
