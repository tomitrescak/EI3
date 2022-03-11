﻿
using System;
using System.Linq;
using Ei.Core.Runtime;

namespace Ei.Core.Ontology
{


    public abstract class Role : RelationalEntity
    {
        private SearchableState resources;

        public Role(string id): base(id) { }

        public abstract SearchableState CreateState();

        public virtual SearchableState Resources {
            get {
                if (this.resources == null) {
                    this.resources = this.CreateState();
                }
                return this.resources;
            }
        }
    }
}