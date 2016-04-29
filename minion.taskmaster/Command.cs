/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

using System.Collections.Generic;

namespace minion.taskmaster
{
    public class Command
    {
        public string File { get; set; }
        public IEnumerable<string> Arguments { get; set; }
    }
}
