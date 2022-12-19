using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    public enum IAPProductType
    {
        Consumable,
        NonConsumable,
        Subscription
    }

    /// <summary>
    /// Represents an in-app product in the Easy Mobile APIs. This class is
    /// different from <see cref="UnityPurchasing.Product"/> but both
    /// are programmatic representations of real world in-app products.
    /// </summary>
    [System.Serializable]
    public class IAPProduct
    {
        /// <summary>
        /// Product name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get { return _name; } }

        /// <summary>
        /// The unified Id of the product.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get { return _id; } }

        /// <summary>
        /// Product type.
        /// </summary>
        /// <value>The type.</value>
        public IAPProductType Type { get { return _type; } }

        /// <summary>
        /// Product price.
        /// </summary>
        /// <value>The price.</value>
        public string Price
        {
            get { return _price; }
            set { _price = value; }
        }

        /// <summary>
        /// Product description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Store-specific product Ids, these Ids if given will override the unified Id for the corresponding stores.
        /// </summary>
        /// <value>The store specific identifiers.</value>
        public StoreSpecificId[] StoreSpecificIds { get { return _storeSpecificIds; } }

        // Required
        [SerializeField]
        private string _name = null;
        [SerializeField]
        private IAPProductType _type = IAPProductType.Consumable;
        [SerializeField]
        private string _id = null;

        // Optional
        [SerializeField]
        private string _price = null;
        [SerializeField]
        private string _description = null;
        [SerializeField]
        private StoreSpecificId[] _storeSpecificIds = null;

#if UNITY_EDITOR
        // Editor-use via reflection only, hence the warning suppression.
#pragma warning disable 0414
        [SerializeField]
        private bool _isEditingAdvanced = false;
#pragma warning restore 0414
#endif

        [System.Serializable]
        public class StoreSpecificId
        {
            public IAPStore store;
            public string id;

            internal StoreSpecificId() { }

            public StoreSpecificId(IAPStore store, string id)
            {
                this.store = store;
                this.id = id;
            }
        }

        internal IAPProduct() { }

        /// <summary>
        /// Creates a new IAP product definition.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="price"></param>
        /// <param name="description"></param>
        public IAPProduct(string id, string name, IAPProductType type, string price, string description = null)
        {
            _id = id;
            _name = name;
            _type = type;
            _price = price;
            _description = description;
        }

        /// <summary>
        /// Creates a new IAP product definition.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeSpecificIds"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="price"></param>
        /// <param name="description"></param>
        public IAPProduct(string id, StoreSpecificId[] storeSpecificIds, string name, IAPProductType type, string price, string description = null) : this(id, name, type, price, description)
        {
            _storeSpecificIds = storeSpecificIds;
        }
    }
}

