using UnityEngine;
using System.Collections;

namespace GOAP {
    public class Condition {

        public string identifier;
        public object value;

        public Condition(string identifier, object value) {
            this.identifier = identifier;
            this.value = value;
        }
    }
}
