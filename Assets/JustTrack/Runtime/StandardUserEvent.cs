using System.Collections;
using System.Collections.Generic;

namespace JustTrack {
    public class StandardUserEvent: UserEventBase {
        internal StandardUserEvent(EventDetails pName) : base(pName) {
        }

        internal override string GetNameFromParts() {
            if ($"{Name.Category}_{Name.Element}_{Name.Action}" == Name.Name) {
                return Name.Name;
            }

            return "";
        }
    }
}