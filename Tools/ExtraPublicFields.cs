using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SpecialStuffPack
{
    public static class ExtraPublicFields
    {
        public static List<ShopItemController> ItemControllers(this BaseShopController shop)
        {
            return (List<ShopItemController>)typeof(BaseShopController).GetField("m_itemControllers", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(shop);
        }

        public static BaseShopController BaseParentShop(this ShopItemController shop)
        {
            return (BaseShopController)typeof(ShopItemController).GetField("m_baseParentShop", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(shop);
        }
    }
}
