
var a = @{
    aaa : 100,
    bbb :200,
}
var b = {
    aaa : 100,
    bbb : 200,
    222 : 300
}
foreach (var pair in pairs(a)) {
    print(pair.key, pair.value)
}
foreach (var pair in pairs(b)) {
    print(pair.key, pair.value)
}
a.forEach((key, value) => {
    print("------------ " + key + "    " + value)
})
b.forEach((key, value) => {
    print("------------ " + key + "    " + value)
})
print(io.unixNow())

// var a = class {

// }
// var b = new a()
// print(b)
// class VersionData {
//     constructor(version) {
//         this.version = version
//         this.versionNumer = this.GetVersionNumber(version)
//     }
//     GetVersionNumber(version) {
//         var versions = version.split ('.');
//         return toNumber(versions[0]) * 1000000 + toNumber(versions[1]) * 10000 + toNumber (versions[2]);
//     }
//     ">"(version) {
//         if (isNumber(version)) {
//             return this.versionNumer > version
//         } else {
//             return this.versionNumer > this.GetVersionNumber(version)
//         }
//     }
//     ">="(version) {
//         if (isNumber(version)) {
//             return this.versionNumer >= version
//         } else {
//             return this.versionNumer >= this.GetVersionNumber(version)
//         }
//     }
//     "<"(version) {
//         if (isNumber(version)) {
//             return this.versionNumer < version
//         } else {
//             return this.versionNumer < this.GetVersionNumber(version)
//         }
//     }
//     "<="(version) {
//         if (isNumber(version)) {
//             return this.versionNumer <= version
//         } else {
//             return this.versionNumer <= this.GetVersionNumber(version)
//         }
//     }
//     "=="(version) {
//         if (isNumber(version)) {
//             return this.versionNumer == version
//         } else {
//             return this.version == version
//         }
//     }
// }
// var t = new VersionData("1.0.13")
// print(t > "0.1.20")
// // TableManager = {}
// // TableManager.getAchievement = function(ID) {
// // }
// // TableManager = {
// //     getAchievement(ID) {
// //         if (this._TableAchievement == null) {
// //             this._TableAchievement = TableAchievement().Initialize('Achievement', GetReader('Achievement'))
// //         }
// //         return ID == null ?  this._TableAchievement : this._TableAchievement(ID)
// //     }
// //     getAdornment(ID) {
// //         if (this._TableAdornment == null) {
// //             this._TableAdornment = TableAdornment().Initialize('Adornment', GetReader('Adornment'))
// //         }
// //         return ID == null ?  this._TableAdornment : this._TableAdornment(ID)
// //     }
// //     getAnimal(ID) {
// //         if (this._TableAnimal == null) {
// //             this._TableAnimal = TableAnimal().Initialize('Animal', GetReader('Animal'))
// //         }
// //         return ID == null ?  this._TableAnimal : this._TableAnimal(ID)
// //     }
// //     getAnimalRatio(ID) {
// //         if (this._TableAnimalRatio == null) {
// //             this._TableAnimalRatio = TableAnimalRatio().Initialize('AnimalRatio', GetReader('AnimalRatio'))
// //         }
// //         return ID == null ?  this._TableAnimalRatio : this._TableAnimalRatio(ID)
// //     }
// //     getAnimalType(ID) {
// //         if (this._TableAnimalType == null) {
// //             this._TableAnimalType = TableAnimalType().Initialize('AnimalType', GetReader('AnimalType'))
// //         }
// //         return ID == null ?  this._TableAnimalType : this._TableAnimalType(ID)
// //     }
// //     getArtifact(ID) {
// //         if (this._TableArtifact == null) {
// //             this._TableArtifact = TableArtifact().Initialize('Artifact', GetReader('Artifact'))
// //         }
// //         return ID == null ?  this._TableArtifact : this._TableArtifact(ID)
// //     }
// //     getArtifactItem(ID) {
// //         if (this._TableArtifactItem == null) {
// //             this._TableArtifactItem = TableArtifactItem().Initialize('ArtifactItem', GetReader('ArtifactItem'))
// //         }
// //         return ID == null ?  this._TableArtifactItem : this._TableArtifactItem(ID)
// //     }
// //     getArtifactItemByGroup(groupID) {
// //         if (this.cacheArtifactItemGroups == null) {
// //             this.cacheArtifactItemGroups = {}
// //             this.getArtifactItem().Datas().forEach((key, value) => {
// //                 var array = this.cacheArtifactItemGroups[value.ArtifactID] ?? (this.cacheArtifactItemGroups[value.ArtifactID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheArtifactItemGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.ItemID - b.ItemID })
// //             })
// //         }
// //         return groupID == null ? this.cacheArtifactItemGroups : (this.cacheArtifactItemGroups[groupID] ?? log.warn("ArtifactItem Group not found GroupID : " + groupID))
// //     }
// //     getBlocker(ID) {
// //         if (this._TableBlocker == null) {
// //             this._TableBlocker = TableBlocker().Initialize('Blocker', GetReader('Blocker'))
// //         }
// //         return ID == null ?  this._TableBlocker : this._TableBlocker(ID)
// //     }
// //     getBlockerType(ID) {
// //         if (this._TableBlockerType == null) {
// //             this._TableBlockerType = TableBlockerType().Initialize('BlockerType', GetReader('BlockerType'))
// //         }
// //         return ID == null ?  this._TableBlockerType : this._TableBlockerType(ID)
// //     }
// //     getBomb(ID) {
// //         if (this._TableBomb == null) {
// //             this._TableBomb = TableBomb().Initialize('Bomb', GetReader('Bomb'))
// //         }
// //         return ID == null ?  this._TableBomb : this._TableBomb(ID)
// //     }
// //     getBuff(ID) {
// //         if (this._TableBuff == null) {
// //             this._TableBuff = TableBuff().Initialize('Buff', GetReader('Buff'))
// //         }
// //         return ID == null ?  this._TableBuff : this._TableBuff(ID)
// //     }
// //     getBuilding(ID) {
// //         if (this._TableBuilding == null) {
// //             this._TableBuilding = TableBuilding().Initialize('Building', GetReader('Building'))
// //         }
// //         return ID == null ?  this._TableBuilding : this._TableBuilding(ID)
// //     }
// //     getCommunity(ID) {
// //         if (this._TableCommunity == null) {
// //             this._TableCommunity = TableCommunity().Initialize('Community', GetReader('Community'))
// //         }
// //         return ID == null ?  this._TableCommunity : this._TableCommunity(ID)
// //     }
// //     getConst(ID) {
// //         if (this._TableConst == null) {
// //             this._TableConst = TableConst().Initialize('Const', GetReader('Const'))
// //         }
// //         return ID == null ?  this._TableConst : this._TableConst(ID)
// //     }
// //     getConstruction(ID) {
// //         if (this._TableConstruction == null) {
// //             this._TableConstruction = TableConstruction().Initialize('Construction', GetReader('Construction'))
// //         }
// //         return ID == null ?  this._TableConstruction : this._TableConstruction(ID)
// //     }
// //     getConstructionCondition(ID) {
// //         if (this._TableConstructionCondition == null) {
// //             this._TableConstructionCondition = TableConstructionCondition().Initialize('ConstructionCondition', GetReader('ConstructionCondition'))
// //         }
// //         return ID == null ?  this._TableConstructionCondition : this._TableConstructionCondition(ID)
// //     }
// //     getConstructionConditionByGroup(groupID) {
// //         if (this.cacheConstructionConditionGroups == null) {
// //             this.cacheConstructionConditionGroups = {}
// //             this.getConstructionCondition().Datas().forEach((key, value) => {
// //                 var array = this.cacheConstructionConditionGroups[value.GroupID] ?? (this.cacheConstructionConditionGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //         }
// //         return groupID == null ? this.cacheConstructionConditionGroups : (this.cacheConstructionConditionGroups[groupID] ?? log.warn("ConstructionCondition Group not found GroupID : " + groupID))
// //     }
// //     getConstructionPhase(ID) {
// //         if (this._TableConstructionPhase == null) {
// //             this._TableConstructionPhase = TableConstructionPhase().Initialize('ConstructionPhase', GetReader('ConstructionPhase'))
// //         }
// //         return ID == null ?  this._TableConstructionPhase : this._TableConstructionPhase(ID)
// //     }
// //     getConstructionPhaseByGroup(groupID) {
// //         if (this.cacheConstructionPhaseGroups == null) {
// //             this.cacheConstructionPhaseGroups = {}
// //             this.getConstructionPhase().Datas().forEach((key, value) => {
// //                 var array = this.cacheConstructionPhaseGroups[value.ConstructionID] ?? (this.cacheConstructionPhaseGroups[value.ConstructionID] = [])
// //                 array.add(value)
// //             })
// //         }
// //         return groupID == null ? this.cacheConstructionPhaseGroups : (this.cacheConstructionPhaseGroups[groupID] ?? log.warn("ConstructionPhase Group not found GroupID : " + groupID))
// //     }
// //     getCrop(ID) {
// //         if (this._TableCrop == null) {
// //             this._TableCrop = TableCrop().Initialize('Crop', GetReader('Crop'))
// //         }
// //         return ID == null ?  this._TableCrop : this._TableCrop(ID)
// //     }
// //     getCropByGroup(groupID) {
// //         if (this.cacheCropGroups == null) {
// //             this.cacheCropGroups = {}
// //             this.getCrop().Datas().forEach((key, value) => {
// //                 var array = this.cacheCropGroups[value.FieldID] ?? (this.cacheCropGroups[value.FieldID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheCropGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.Level - b.Level })
// //             })
// //         }
// //         return groupID == null ? this.cacheCropGroups : (this.cacheCropGroups[groupID] ?? log.warn("Crop Group not found GroupID : " + groupID))
// //     }
// //     getDealer(ID) {
// //         if (this._TableDealer == null) {
// //             this._TableDealer = TableDealer().Initialize('Dealer', GetReader('Dealer'))
// //         }
// //         return ID == null ?  this._TableDealer : this._TableDealer(ID)
// //     }
// //     getDealerSlot(ID) {
// //         if (this._TableDealerSlot == null) {
// //             this._TableDealerSlot = TableDealerSlot().Initialize('DealerSlot', GetReader('DealerSlot'))
// //         }
// //         return ID == null ?  this._TableDealerSlot : this._TableDealerSlot(ID)
// //     }
// //     getDealerSlotByGroup(groupID) {
// //         if (this.cacheDealerSlotGroups == null) {
// //             this.cacheDealerSlotGroups = {}
// //             this.getDealerSlot().Datas().forEach((key, value) => {
// //                 var array = this.cacheDealerSlotGroups[value.DealerID] ?? (this.cacheDealerSlotGroups[value.DealerID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheDealerSlotGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.ID - b.ID })
// //             })
// //         }
// //         return groupID == null ? this.cacheDealerSlotGroups : (this.cacheDealerSlotGroups[groupID] ?? log.warn("DealerSlot Group not found GroupID : " + groupID))
// //     }
// //     getDecoration(ID) {
// //         if (this._TableDecoration == null) {
// //             this._TableDecoration = TableDecoration().Initialize('Decoration', GetReader('Decoration'))
// //         }
// //         return ID == null ?  this._TableDecoration : this._TableDecoration(ID)
// //     }
// //     getDragon(ID) {
// //         if (this._TableDragon == null) {
// //             this._TableDragon = TableDragon().Initialize('Dragon', GetReader('Dragon'))
// //         }
// //         return ID == null ?  this._TableDragon : this._TableDragon(ID)
// //     }
// //     getDragonGroup(ID) {
// //         if (this._TableDragonGroup == null) {
// //             this._TableDragonGroup = TableDragonGroup().Initialize('DragonGroup', GetReader('DragonGroup'))
// //         }
// //         return ID == null ?  this._TableDragonGroup : this._TableDragonGroup(ID)
// //     }
// //     getDragonHome(ID) {
// //         if (this._TableDragonHome == null) {
// //             this._TableDragonHome = TableDragonHome().Initialize('DragonHome', GetReader('DragonHome'))
// //         }
// //         return ID == null ?  this._TableDragonHome : this._TableDragonHome(ID)
// //     }
// //     getDragonTree(ID) {
// //         if (this._TableDragonTree == null) {
// //             this._TableDragonTree = TableDragonTree().Initialize('DragonTree', GetReader('DragonTree'))
// //         }
// //         return ID == null ?  this._TableDragonTree : this._TableDragonTree(ID)
// //     }
// //     getDragonTreeType(ID) {
// //         if (this._TableDragonTreeType == null) {
// //             this._TableDragonTreeType = TableDragonTreeType().Initialize('DragonTreeType', GetReader('DragonTreeType'))
// //         }
// //         return ID == null ?  this._TableDragonTreeType : this._TableDragonTreeType(ID)
// //     }
// //     getEnergy(ID) {
// //         if (this._TableEnergy == null) {
// //             this._TableEnergy = TableEnergy().Initialize('Energy', GetReader('Energy'))
// //         }
// //         return ID == null ?  this._TableEnergy : this._TableEnergy(ID)
// //     }
// //     getEnergyExchange(ID) {
// //         if (this._TableEnergyExchange == null) {
// //             this._TableEnergyExchange = TableEnergyExchange().Initialize('EnergyExchange', GetReader('EnergyExchange'))
// //         }
// //         return ID == null ?  this._TableEnergyExchange : this._TableEnergyExchange(ID)
// //     }
// //     getEntrance(ID) {
// //         if (this._TableEntrance == null) {
// //             this._TableEntrance = TableEntrance().Initialize('Entrance', GetReader('Entrance'))
// //         }
// //         return ID == null ?  this._TableEntrance : this._TableEntrance(ID)
// //     }
// //     getExchangeFactory(ID) {
// //         if (this._TableExchangeFactory == null) {
// //             this._TableExchangeFactory = TableExchangeFactory().Initialize('ExchangeFactory', GetReader('ExchangeFactory'))
// //         }
// //         return ID == null ?  this._TableExchangeFactory : this._TableExchangeFactory(ID)
// //     }
// //     getExchangeFactoryItem(ID) {
// //         if (this._TableExchangeFactoryItem == null) {
// //             this._TableExchangeFactoryItem = TableExchangeFactoryItem().Initialize('ExchangeFactoryItem', GetReader('ExchangeFactoryItem'))
// //         }
// //         return ID == null ?  this._TableExchangeFactoryItem : this._TableExchangeFactoryItem(ID)
// //     }
// //     getExchangeFactoryItemByGroup(groupID) {
// //         if (this.cacheExchangeFactoryItemGroups == null) {
// //             this.cacheExchangeFactoryItemGroups = {}
// //             this.getExchangeFactoryItem().Datas().forEach((key, value) => {
// //                 var array = this.cacheExchangeFactoryItemGroups[value.GroupID] ?? (this.cacheExchangeFactoryItemGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheExchangeFactoryItemGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.ItemID - b.ItemID })
// //             })
// //         }
// //         return groupID == null ? this.cacheExchangeFactoryItemGroups : (this.cacheExchangeFactoryItemGroups[groupID] ?? log.warn("ExchangeFactoryItem Group not found GroupID : " + groupID))
// //     }
// //     getExpansion(ID) {
// //         if (this._TableExpansion == null) {
// //             this._TableExpansion = TableExpansion().Initialize('Expansion', GetReader('Expansion'))
// //         }
// //         return ID == null ?  this._TableExpansion : this._TableExpansion(ID)
// //     }
// //     getExpansionByGroup(groupID) {
// //         if (this.cacheExpansionGroups == null) {
// //             this.cacheExpansionGroups = {}
// //             this.getExpansion().Datas().forEach((key, value) => {
// //                 var array = this.cacheExpansionGroups[value.GroupID] ?? (this.cacheExpansionGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //         }
// //         return groupID == null ? this.cacheExpansionGroups : (this.cacheExpansionGroups[groupID] ?? log.warn("Expansion Group not found GroupID : " + groupID))
// //     }
// //     getExpansionCondition(ID) {
// //         if (this._TableExpansionCondition == null) {
// //             this._TableExpansionCondition = TableExpansionCondition().Initialize('ExpansionCondition', GetReader('ExpansionCondition'))
// //         }
// //         return ID == null ?  this._TableExpansionCondition : this._TableExpansionCondition(ID)
// //     }
// //     getExpansionConditionByGroup(groupID) {
// //         if (this.cacheExpansionConditionGroups == null) {
// //             this.cacheExpansionConditionGroups = {}
// //             this.getExpansionCondition().Datas().forEach((key, value) => {
// //                 var array = this.cacheExpansionConditionGroups[value.GroupID] ?? (this.cacheExpansionConditionGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheExpansionConditionGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.RequireValue - b.RequireValue })
// //             })
// //         }
// //         return groupID == null ? this.cacheExpansionConditionGroups : (this.cacheExpansionConditionGroups[groupID] ?? log.warn("ExpansionCondition Group not found GroupID : " + groupID))
// //     }
// //     getExpansionCount(ID) {
// //         if (this._TableExpansionCount == null) {
// //             this._TableExpansionCount = TableExpansionCount().Initialize('ExpansionCount', GetReader('ExpansionCount'))
// //         }
// //         return ID == null ?  this._TableExpansionCount : this._TableExpansionCount(ID)
// //     }
// //     getExpansionCountByGroup(groupID) {
// //         if (this.cacheExpansionCountGroups == null) {
// //             this.cacheExpansionCountGroups = {}
// //             this.getExpansionCount().Datas().forEach((key, value) => {
// //                 var array = this.cacheExpansionCountGroups[value.GroupID] ?? (this.cacheExpansionCountGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheExpansionCountGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.ExpansionCount - b.ExpansionCount })
// //             })
// //         }
// //         return groupID == null ? this.cacheExpansionCountGroups : (this.cacheExpansionCountGroups[groupID] ?? log.warn("ExpansionCount Group not found GroupID : " + groupID))
// //     }
// //     getFacility(ID) {
// //         if (this._TableFacility == null) {
// //             this._TableFacility = TableFacility().Initialize('Facility', GetReader('Facility'))
// //         }
// //         return ID == null ?  this._TableFacility : this._TableFacility(ID)
// //     }
// //     getFacilityList(ID) {
// //         if (this._TableFacilityList == null) {
// //             this._TableFacilityList = TableFacilityList().Initialize('FacilityList', GetReader('FacilityList'))
// //         }
// //         return ID == null ?  this._TableFacilityList : this._TableFacilityList(ID)
// //     }
// //     getFacilityListByGroup(groupID) {
// //         if (this.cacheFacilityListGroups == null) {
// //             this.cacheFacilityListGroups = {}
// //             this.getFacilityList().Datas().forEach((key, value) => {
// //                 var array = this.cacheFacilityListGroups[value.FacilityGroupID] ?? (this.cacheFacilityListGroups[value.FacilityGroupID] = [])
// //                 array.add(value)
// //             })
// //         }
// //         return groupID == null ? this.cacheFacilityListGroups : (this.cacheFacilityListGroups[groupID] ?? log.warn("FacilityList Group not found GroupID : " + groupID))
// //     }
// //     getFacilitySlot(ID) {
// //         if (this._TableFacilitySlot == null) {
// //             this._TableFacilitySlot = TableFacilitySlot().Initialize('FacilitySlot', GetReader('FacilitySlot'))
// //         }
// //         return ID == null ?  this._TableFacilitySlot : this._TableFacilitySlot(ID)
// //     }
// //     getFacilitySlotByGroup(groupID) {
// //         if (this.cacheFacilitySlotGroups == null) {
// //             this.cacheFacilitySlotGroups = {}
// //             this.getFacilitySlot().Datas().forEach((key, value) => {
// //                 var array = this.cacheFacilitySlotGroups[value.FacilityGroupID] ?? (this.cacheFacilitySlotGroups[value.FacilityGroupID] = [])
// //                 array.add(value)
// //             })
// //         }
// //         return groupID == null ? this.cacheFacilitySlotGroups : (this.cacheFacilitySlotGroups[groupID] ?? log.warn("FacilitySlot Group not found GroupID : " + groupID))
// //     }
// //     getFactory(ID) {
// //         if (this._TableFactory == null) {
// //             this._TableFactory = TableFactory().Initialize('Factory', GetReader('Factory'))
// //         }
// //         return ID == null ?  this._TableFactory : this._TableFactory(ID)
// //     }
// //     getFactoryProduct(ID) {
// //         if (this._TableFactoryProduct == null) {
// //             this._TableFactoryProduct = TableFactoryProduct().Initialize('FactoryProduct', GetReader('FactoryProduct'))
// //         }
// //         return ID == null ?  this._TableFactoryProduct : this._TableFactoryProduct(ID)
// //     }
// //     getFactoryProductByGroup(groupID) {
// //         if (this.cacheFactoryProductGroups == null) {
// //             this.cacheFactoryProductGroups = {}
// //             this.getFactoryProduct().Datas().forEach((key, value) => {
// //                 var array = this.cacheFactoryProductGroups[value.FactoryID] ?? (this.cacheFactoryProductGroups[value.FactoryID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheFactoryProductGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.Level - b.Level })
// //             })
// //         }
// //         return groupID == null ? this.cacheFactoryProductGroups : (this.cacheFactoryProductGroups[groupID] ?? log.warn("FactoryProduct Group not found GroupID : " + groupID))
// //     }
// //     getFactoryProductStock(ID) {
// //         if (this._TableFactoryProductStock == null) {
// //             this._TableFactoryProductStock = TableFactoryProductStock().Initialize('FactoryProductStock', GetReader('FactoryProductStock'))
// //         }
// //         return ID == null ?  this._TableFactoryProductStock : this._TableFactoryProductStock(ID)
// //     }
// //     getFactoryProductStockByGroup(groupID) {
// //         if (this.cacheFactoryProductStockGroups == null) {
// //             this.cacheFactoryProductStockGroups = {}
// //             this.getFactoryProductStock().Datas().forEach((key, value) => {
// //                 var array = this.cacheFactoryProductStockGroups[value.GroupID] ?? (this.cacheFactoryProductStockGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheFactoryProductStockGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.ItemID - b.ItemID })
// //             })
// //         }
// //         return groupID == null ? this.cacheFactoryProductStockGroups : (this.cacheFactoryProductStockGroups[groupID] ?? log.warn("FactoryProductStock Group not found GroupID : " + groupID))
// //     }
// //     getFactorySlotIncrease(ID) {
// //         if (this._TableFactorySlotIncrease == null) {
// //             this._TableFactorySlotIncrease = TableFactorySlotIncrease().Initialize('FactorySlotIncrease', GetReader('FactorySlotIncrease'))
// //         }
// //         return ID == null ?  this._TableFactorySlotIncrease : this._TableFactorySlotIncrease(ID)
// //     }
// //     getFactorySlotIncreaseByGroup(groupID) {
// //         if (this.cacheFactorySlotIncreaseGroups == null) {
// //             this.cacheFactorySlotIncreaseGroups = {}
// //             this.getFactorySlotIncrease().Datas().forEach((key, value) => {
// //                 var array = this.cacheFactorySlotIncreaseGroups[value.GroupID] ?? (this.cacheFactorySlotIncreaseGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //         }
// //         return groupID == null ? this.cacheFactorySlotIncreaseGroups : (this.cacheFactorySlotIncreaseGroups[groupID] ?? log.warn("FactorySlotIncrease Group not found GroupID : " + groupID))
// //     }
// //     getFeatureGuide(ID) {
// //         if (this._TableFeatureGuide == null) {
// //             this._TableFeatureGuide = TableFeatureGuide().Initialize('FeatureGuide', GetReader('FeatureGuide'))
// //         }
// //         return ID == null ?  this._TableFeatureGuide : this._TableFeatureGuide(ID)
// //     }
// //     getFeatureTip(ID) {
// //         if (this._TableFeatureTip == null) {
// //             this._TableFeatureTip = TableFeatureTip().Initialize('FeatureTip', GetReader('FeatureTip'))
// //         }
// //         return ID == null ?  this._TableFeatureTip : this._TableFeatureTip(ID)
// //     }
// //     getField(ID) {
// //         if (this._TableField == null) {
// //             this._TableField = TableField().Initialize('Field', GetReader('Field'))
// //         }
// //         return ID == null ?  this._TableField : this._TableField(ID)
// //     }
// //     getFunctionBuilding(ID) {
// //         if (this._TableFunctionBuilding == null) {
// //             this._TableFunctionBuilding = TableFunctionBuilding().Initialize('FunctionBuilding', GetReader('FunctionBuilding'))
// //         }
// //         return ID == null ?  this._TableFunctionBuilding : this._TableFunctionBuilding(ID)
// //     }
// //     getIcon(ID) {
// //         if (this._TableIcon == null) {
// //             this._TableIcon = TableIcon().Initialize('Icon', GetReader('Icon'))
// //         }
// //         return ID == null ?  this._TableIcon : this._TableIcon(ID)
// //     }
// //     getItem(ID) {
// //         if (this._TableItem == null) {
// //             this._TableItem = TableItem().Initialize('Item', GetReader('Item'))
// //         }
// //         return ID == null ?  this._TableItem : this._TableItem(ID)
// //     }
// //     getl10n_de(ID) {
// //         if (this._Tablel10n_de == null) {
// //             this._Tablel10n_de = Tablel10n_de().Initialize('l10n_de', GetReader('l10n_de'))
// //         }
// //         return ID == null ?  this._Tablel10n_de : this._Tablel10n_de(ID)
// //     }
// //     getl10n_en(ID) {
// //         if (this._Tablel10n_en == null) {
// //             this._Tablel10n_en = Tablel10n_en().Initialize('l10n_en', GetReader('l10n_en'))
// //         }
// //         return ID == null ?  this._Tablel10n_en : this._Tablel10n_en(ID)
// //     }
// //     getl10n_es(ID) {
// //         if (this._Tablel10n_es == null) {
// //             this._Tablel10n_es = Tablel10n_es().Initialize('l10n_es', GetReader('l10n_es'))
// //         }
// //         return ID == null ?  this._Tablel10n_es : this._Tablel10n_es(ID)
// //     }
// //     getl10n_fr(ID) {
// //         if (this._Tablel10n_fr == null) {
// //             this._Tablel10n_fr = Tablel10n_fr().Initialize('l10n_fr', GetReader('l10n_fr'))
// //         }
// //         return ID == null ?  this._Tablel10n_fr : this._Tablel10n_fr(ID)
// //     }
// //     getl10n_id(ID) {
// //         if (this._Tablel10n_id == null) {
// //             this._Tablel10n_id = Tablel10n_id().Initialize('l10n_id', GetReader('l10n_id'))
// //         }
// //         return ID == null ?  this._Tablel10n_id : this._Tablel10n_id(ID)
// //     }
// //     getl10n_it(ID) {
// //         if (this._Tablel10n_it == null) {
// //             this._Tablel10n_it = Tablel10n_it().Initialize('l10n_it', GetReader('l10n_it'))
// //         }
// //         return ID == null ?  this._Tablel10n_it : this._Tablel10n_it(ID)
// //     }
// //     getl10n_jp(ID) {
// //         if (this._Tablel10n_jp == null) {
// //             this._Tablel10n_jp = Tablel10n_jp().Initialize('l10n_jp', GetReader('l10n_jp'))
// //         }
// //         return ID == null ?  this._Tablel10n_jp : this._Tablel10n_jp(ID)
// //     }
// //     getl10n_ko(ID) {
// //         if (this._Tablel10n_ko == null) {
// //             this._Tablel10n_ko = Tablel10n_ko().Initialize('l10n_ko', GetReader('l10n_ko'))
// //         }
// //         return ID == null ?  this._Tablel10n_ko : this._Tablel10n_ko(ID)
// //     }
// //     getl10n_nl(ID) {
// //         if (this._Tablel10n_nl == null) {
// //             this._Tablel10n_nl = Tablel10n_nl().Initialize('l10n_nl', GetReader('l10n_nl'))
// //         }
// //         return ID == null ?  this._Tablel10n_nl : this._Tablel10n_nl(ID)
// //     }
// //     getl10n_pl(ID) {
// //         if (this._Tablel10n_pl == null) {
// //             this._Tablel10n_pl = Tablel10n_pl().Initialize('l10n_pl', GetReader('l10n_pl'))
// //         }
// //         return ID == null ?  this._Tablel10n_pl : this._Tablel10n_pl(ID)
// //     }
// //     getl10n_pt(ID) {
// //         if (this._Tablel10n_pt == null) {
// //             this._Tablel10n_pt = Tablel10n_pt().Initialize('l10n_pt', GetReader('l10n_pt'))
// //         }
// //         return ID == null ?  this._Tablel10n_pt : this._Tablel10n_pt(ID)
// //     }
// //     getl10n_ru(ID) {
// //         if (this._Tablel10n_ru == null) {
// //             this._Tablel10n_ru = Tablel10n_ru().Initialize('l10n_ru', GetReader('l10n_ru'))
// //         }
// //         return ID == null ?  this._Tablel10n_ru : this._Tablel10n_ru(ID)
// //     }
// //     getl10n_sc(ID) {
// //         if (this._Tablel10n_sc == null) {
// //             this._Tablel10n_sc = Tablel10n_sc().Initialize('l10n_sc', GetReader('l10n_sc'))
// //         }
// //         return ID == null ?  this._Tablel10n_sc : this._Tablel10n_sc(ID)
// //     }
// //     getl10n_tc(ID) {
// //         if (this._Tablel10n_tc == null) {
// //             this._Tablel10n_tc = Tablel10n_tc().Initialize('l10n_tc', GetReader('l10n_tc'))
// //         }
// //         return ID == null ?  this._Tablel10n_tc : this._Tablel10n_tc(ID)
// //     }
// //     getl10n_th(ID) {
// //         if (this._Tablel10n_th == null) {
// //             this._Tablel10n_th = Tablel10n_th().Initialize('l10n_th', GetReader('l10n_th'))
// //         }
// //         return ID == null ?  this._Tablel10n_th : this._Tablel10n_th(ID)
// //     }
// //     getl10n_tr(ID) {
// //         if (this._Tablel10n_tr == null) {
// //             this._Tablel10n_tr = Tablel10n_tr().Initialize('l10n_tr', GetReader('l10n_tr'))
// //         }
// //         return ID == null ?  this._Tablel10n_tr : this._Tablel10n_tr(ID)
// //     }
// //     getLevel(ID) {
// //         if (this._TableLevel == null) {
// //             this._TableLevel = TableLevel().Initialize('Level', GetReader('Level'))
// //         }
// //         return ID == null ?  this._TableLevel : this._TableLevel(ID)
// //     }
// //     getLoadingTip(ID) {
// //         if (this._TableLoadingTip == null) {
// //             this._TableLoadingTip = TableLoadingTip().Initialize('LoadingTip', GetReader('LoadingTip'))
// //         }
// //         return ID == null ?  this._TableLoadingTip : this._TableLoadingTip(ID)
// //     }
// //     getLogin30(ID) {
// //         if (this._TableLogin30 == null) {
// //             this._TableLogin30 = TableLogin30().Initialize('Login30', GetReader('Login30'))
// //         }
// //         return ID == null ?  this._TableLogin30 : this._TableLogin30(ID)
// //     }
// //     getLogin30ByGroup(groupID) {
// //         if (this.cacheLogin30Groups == null) {
// //             this.cacheLogin30Groups = {}
// //             this.getLogin30().Datas().forEach((key, value) => {
// //                 var array = this.cacheLogin30Groups[value.GroupID] ?? (this.cacheLogin30Groups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //         }
// //         return groupID == null ? this.cacheLogin30Groups : (this.cacheLogin30Groups[groupID] ?? log.warn("Login30 Group not found GroupID : " + groupID))
// //     }
// //     getLogin7New(ID) {
// //         if (this._TableLogin7New == null) {
// //             this._TableLogin7New = TableLogin7New().Initialize('Login7New', GetReader('Login7New'))
// //         }
// //         return ID == null ?  this._TableLogin7New : this._TableLogin7New(ID)
// //     }
// //     getLogin7NewByGroup(groupID) {
// //         if (this.cacheLogin7NewGroups == null) {
// //             this.cacheLogin7NewGroups = {}
// //             this.getLogin7New().Datas().forEach((key, value) => {
// //                 var array = this.cacheLogin7NewGroups[value.GroupID] ?? (this.cacheLogin7NewGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //         }
// //         return groupID == null ? this.cacheLogin7NewGroups : (this.cacheLogin7NewGroups[groupID] ?? log.warn("Login7New Group not found GroupID : " + groupID))
// //     }
// //     getMap(ID) {
// //         if (this._TableMap == null) {
// //             this._TableMap = TableMap().Initialize('Map', GetReader('Map'))
// //         }
// //         return ID == null ?  this._TableMap : this._TableMap(ID)
// //     }
// //     getMapChapter(ID) {
// //         if (this._TableMapChapter == null) {
// //             this._TableMapChapter = TableMapChapter().Initialize('MapChapter', GetReader('MapChapter'))
// //         }
// //         return ID == null ?  this._TableMapChapter : this._TableMapChapter(ID)
// //     }
// //     getMapGear(ID) {
// //         if (this._TableMapGear == null) {
// //             this._TableMapGear = TableMapGear().Initialize('MapGear', GetReader('MapGear'))
// //         }
// //         return ID == null ?  this._TableMapGear : this._TableMapGear(ID)
// //     }
// //     getMapGearState(ID) {
// //         if (this._TableMapGearState == null) {
// //             this._TableMapGearState = TableMapGearState().Initialize('MapGearState', GetReader('MapGearState'))
// //         }
// //         return ID == null ?  this._TableMapGearState : this._TableMapGearState(ID)
// //     }
// //     getMapQuest(ID) {
// //         if (this._TableMapQuest == null) {
// //             this._TableMapQuest = TableMapQuest().Initialize('MapQuest', GetReader('MapQuest'))
// //         }
// //         return ID == null ?  this._TableMapQuest : this._TableMapQuest(ID)
// //     }
// //     getMapQuestFlag(ID) {
// //         if (this._TableMapQuestFlag == null) {
// //             this._TableMapQuestFlag = TableMapQuestFlag().Initialize('MapQuestFlag', GetReader('MapQuestFlag'))
// //         }
// //         return ID == null ?  this._TableMapQuestFlag : this._TableMapQuestFlag(ID)
// //     }
// //     getMapQuestFlagByGroup(groupID) {
// //         if (this.cacheMapQuestFlagGroups == null) {
// //             this.cacheMapQuestFlagGroups = {}
// //             this.getMapQuestFlag().Datas().forEach((key, value) => {
// //                 var array = this.cacheMapQuestFlagGroups[value.QuestID] ?? (this.cacheMapQuestFlagGroups[value.QuestID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheMapQuestFlagGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.ID - b.ID })
// //             })
// //         }
// //         return groupID == null ? this.cacheMapQuestFlagGroups : (this.cacheMapQuestFlagGroups[groupID] ?? log.warn("MapQuestFlag Group not found GroupID : " + groupID))
// //     }
// //     getMonthlyCard(ID) {
// //         if (this._TableMonthlyCard == null) {
// //             this._TableMonthlyCard = TableMonthlyCard().Initialize('MonthlyCard', GetReader('MonthlyCard'))
// //         }
// //         return ID == null ?  this._TableMonthlyCard : this._TableMonthlyCard(ID)
// //     }
// //     getMonthlyCardByGroup(groupID) {
// //         if (this.cacheMonthlyCardGroups == null) {
// //             this.cacheMonthlyCardGroups = {}
// //             this.getMonthlyCard().Datas().forEach((key, value) => {
// //                 var array = this.cacheMonthlyCardGroups[value.GroupID] ?? (this.cacheMonthlyCardGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheMonthlyCardGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.ID - b.ID })
// //             })
// //         }
// //         return groupID == null ? this.cacheMonthlyCardGroups : (this.cacheMonthlyCardGroups[groupID] ?? log.warn("MonthlyCard Group not found GroupID : " + groupID))
// //     }
// //     getNPC(ID) {
// //         if (this._TableNPC == null) {
// //             this._TableNPC = TableNPC().Initialize('NPC', GetReader('NPC'))
// //         }
// //         return ID == null ?  this._TableNPC : this._TableNPC(ID)
// //     }
// //     getNPCTip(ID) {
// //         if (this._TableNPCTip == null) {
// //             this._TableNPCTip = TableNPCTip().Initialize('NPCTip', GetReader('NPCTip'))
// //         }
// //         return ID == null ?  this._TableNPCTip : this._TableNPCTip(ID)
// //     }
// //     getOnlineGift(ID) {
// //         if (this._TableOnlineGift == null) {
// //             this._TableOnlineGift = TableOnlineGift().Initialize('OnlineGift', GetReader('OnlineGift'))
// //         }
// //         return ID == null ?  this._TableOnlineGift : this._TableOnlineGift(ID)
// //     }
// //     getOrderNPC(ID) {
// //         if (this._TableOrderNPC == null) {
// //             this._TableOrderNPC = TableOrderNPC().Initialize('OrderNPC', GetReader('OrderNPC'))
// //         }
// //         return ID == null ?  this._TableOrderNPC : this._TableOrderNPC(ID)
// //     }
// //     getPackage(ID) {
// //         if (this._TablePackage == null) {
// //             this._TablePackage = TablePackage().Initialize('Package', GetReader('Package'))
// //         }
// //         return ID == null ?  this._TablePackage : this._TablePackage(ID)
// //     }
// //     getPackageByGroup(groupID) {
// //         if (this.cachePackageGroups == null) {
// //             this.cachePackageGroups = {}
// //             this.getPackage().Datas().forEach((key, value) => {
// //                 var array = this.cachePackageGroups[value.GroupID] ?? (this.cachePackageGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //             this.cachePackageGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.Sort - b.Sort })
// //             })
// //         }
// //         return groupID == null ? this.cachePackageGroups : (this.cachePackageGroups[groupID] ?? log.warn("Package Group not found GroupID : " + groupID))
// //     }
// //     getPayment(ID) {
// //         if (this._TablePayment == null) {
// //             this._TablePayment = TablePayment().Initialize('Payment', GetReader('Payment'))
// //         }
// //         return ID == null ?  this._TablePayment : this._TablePayment(ID)
// //     }
// //     getPhaseReward(ID) {
// //         if (this._TablePhaseReward == null) {
// //             this._TablePhaseReward = TablePhaseReward().Initialize('PhaseReward', GetReader('PhaseReward'))
// //         }
// //         return ID == null ?  this._TablePhaseReward : this._TablePhaseReward(ID)
// //     }
// //     getPhaseRewardByGroup(groupID) {
// //         if (this.cachePhaseRewardGroups == null) {
// //             this.cachePhaseRewardGroups = {}
// //             this.getPhaseReward().Datas().forEach((key, value) => {
// //                 var array = this.cachePhaseRewardGroups[value.GroupID] ?? (this.cachePhaseRewardGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //             this.cachePhaseRewardGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.Count - b.Count })
// //             })
// //         }
// //         return groupID == null ? this.cachePhaseRewardGroups : (this.cachePhaseRewardGroups[groupID] ?? log.warn("PhaseReward Group not found GroupID : " + groupID))
// //     }
// //     getPiggyBank(ID) {
// //         if (this._TablePiggyBank == null) {
// //             this._TablePiggyBank = TablePiggyBank().Initialize('PiggyBank', GetReader('PiggyBank'))
// //         }
// //         return ID == null ?  this._TablePiggyBank : this._TablePiggyBank(ID)
// //     }
// //     getPiggyBankByGroup(groupID) {
// //         if (this.cachePiggyBankGroups == null) {
// //             this.cachePiggyBankGroups = {}
// //             this.getPiggyBank().Datas().forEach((key, value) => {
// //                 var array = this.cachePiggyBankGroups[value.GroupID] ?? (this.cachePiggyBankGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //             this.cachePiggyBankGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.ID - b.ID })
// //             })
// //         }
// //         return groupID == null ? this.cachePiggyBankGroups : (this.cachePiggyBankGroups[groupID] ?? log.warn("PiggyBank Group not found GroupID : " + groupID))
// //     }
// //     getPlant(ID) {
// //         if (this._TablePlant == null) {
// //             this._TablePlant = TablePlant().Initialize('Plant', GetReader('Plant'))
// //         }
// //         return ID == null ?  this._TablePlant : this._TablePlant(ID)
// //     }
// //     getPlantFactory(ID) {
// //         if (this._TablePlantFactory == null) {
// //             this._TablePlantFactory = TablePlantFactory().Initialize('PlantFactory', GetReader('PlantFactory'))
// //         }
// //         return ID == null ?  this._TablePlantFactory : this._TablePlantFactory(ID)
// //     }
// //     getPlantTag(ID) {
// //         if (this._TablePlantTag == null) {
// //             this._TablePlantTag = TablePlantTag().Initialize('PlantTag', GetReader('PlantTag'))
// //         }
// //         return ID == null ?  this._TablePlantTag : this._TablePlantTag(ID)
// //     }
// //     getPlantTagByGroup(groupID) {
// //         if (this.cachePlantTagGroups == null) {
// //             this.cachePlantTagGroups = {}
// //             this.getPlantTag().Datas().forEach((key, value) => {
// //                 var array = this.cachePlantTagGroups[value.GroupID] ?? (this.cachePlantTagGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //         }
// //         return groupID == null ? this.cachePlantTagGroups : (this.cachePlantTagGroups[groupID] ?? log.warn("PlantTag Group not found GroupID : " + groupID))
// //     }
// //     getPlantTagType(ID) {
// //         if (this._TablePlantTagType == null) {
// //             this._TablePlantTagType = TablePlantTagType().Initialize('PlantTagType', GetReader('PlantTagType'))
// //         }
// //         return ID == null ?  this._TablePlantTagType : this._TablePlantTagType(ID)
// //     }
// //     getPlantType(ID) {
// //         if (this._TablePlantType == null) {
// //             this._TablePlantType = TablePlantType().Initialize('PlantType', GetReader('PlantType'))
// //         }
// //         return ID == null ?  this._TablePlantType : this._TablePlantType(ID)
// //     }
// //     getPoint(ID) {
// //         if (this._TablePoint == null) {
// //             this._TablePoint = TablePoint().Initialize('Point', GetReader('Point'))
// //         }
// //         return ID == null ?  this._TablePoint : this._TablePoint(ID)
// //     }
// //     getProduceFactory(ID) {
// //         if (this._TableProduceFactory == null) {
// //             this._TableProduceFactory = TableProduceFactory().Initialize('ProduceFactory', GetReader('ProduceFactory'))
// //         }
// //         return ID == null ?  this._TableProduceFactory : this._TableProduceFactory(ID)
// //     }
// //     getPrompt(ID) {
// //         if (this._TablePrompt == null) {
// //             this._TablePrompt = TablePrompt().Initialize('Prompt', GetReader('Prompt'))
// //         }
// //         return ID == null ?  this._TablePrompt : this._TablePrompt(ID)
// //     }
// //     getQuest(ID) {
// //         if (this._TableQuest == null) {
// //             this._TableQuest = TableQuest().Initialize('Quest', GetReader('Quest'))
// //         }
// //         return ID == null ?  this._TableQuest : this._TableQuest(ID)
// //     }
// //     getQuestFlag(ID) {
// //         if (this._TableQuestFlag == null) {
// //             this._TableQuestFlag = TableQuestFlag().Initialize('QuestFlag', GetReader('QuestFlag'))
// //         }
// //         return ID == null ?  this._TableQuestFlag : this._TableQuestFlag(ID)
// //     }
// //     getQuestFlagByGroup(groupID) {
// //         if (this.cacheQuestFlagGroups == null) {
// //             this.cacheQuestFlagGroups = {}
// //             this.getQuestFlag().Datas().forEach((key, value) => {
// //                 var array = this.cacheQuestFlagGroups[value.QuestID] ?? (this.cacheQuestFlagGroups[value.QuestID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheQuestFlagGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.ID - b.ID })
// //             })
// //         }
// //         return groupID == null ? this.cacheQuestFlagGroups : (this.cacheQuestFlagGroups[groupID] ?? log.warn("QuestFlag Group not found GroupID : " + groupID))
// //     }
// //     getSlots(ID) {
// //         if (this._TableSlots == null) {
// //             this._TableSlots = TableSlots().Initialize('Slots', GetReader('Slots'))
// //         }
// //         return ID == null ?  this._TableSlots : this._TableSlots(ID)
// //     }
// //     getSlotsIcon(ID) {
// //         if (this._TableSlotsIcon == null) {
// //             this._TableSlotsIcon = TableSlotsIcon().Initialize('SlotsIcon', GetReader('SlotsIcon'))
// //         }
// //         return ID == null ?  this._TableSlotsIcon : this._TableSlotsIcon(ID)
// //     }
// //     getSlotsReward(ID) {
// //         if (this._TableSlotsReward == null) {
// //             this._TableSlotsReward = TableSlotsReward().Initialize('SlotsReward', GetReader('SlotsReward'))
// //         }
// //         return ID == null ?  this._TableSlotsReward : this._TableSlotsReward(ID)
// //     }
// //     getSpecialPrompt(ID) {
// //         if (this._TableSpecialPrompt == null) {
// //             this._TableSpecialPrompt = TableSpecialPrompt().Initialize('SpecialPrompt', GetReader('SpecialPrompt'))
// //         }
// //         return ID == null ?  this._TableSpecialPrompt : this._TableSpecialPrompt(ID)
// //     }
// //     getStage(ID) {
// //         if (this._TableStage == null) {
// //             this._TableStage = TableStage().Initialize('Stage', GetReader('Stage'))
// //         }
// //         return ID == null ?  this._TableStage : this._TableStage(ID)
// //     }
// //     getStageByGroup(groupID) {
// //         if (this.cacheStageGroups == null) {
// //             this.cacheStageGroups = {}
// //             this.getStage().Datas().forEach((key, value) => {
// //                 var array = this.cacheStageGroups[value.StageID] ?? (this.cacheStageGroups[value.StageID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheStageGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.Sort - b.Sort })
// //             })
// //         }
// //         return groupID == null ? this.cacheStageGroups : (this.cacheStageGroups[groupID] ?? log.warn("Stage Group not found GroupID : " + groupID))
// //     }
// //     getStageTip(ID) {
// //         if (this._TableStageTip == null) {
// //             this._TableStageTip = TableStageTip().Initialize('StageTip', GetReader('StageTip'))
// //         }
// //         return ID == null ?  this._TableStageTip : this._TableStageTip(ID)
// //     }
// //     getStageTipByGroup(groupID) {
// //         if (this.cacheStageTipGroups == null) {
// //             this.cacheStageTipGroups = {}
// //             this.getStageTip().Datas().forEach((key, value) => {
// //                 var array = this.cacheStageTipGroups[value.StageID] ?? (this.cacheStageTipGroups[value.StageID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheStageTipGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.ID - b.ID })
// //             })
// //         }
// //         return groupID == null ? this.cacheStageTipGroups : (this.cacheStageTipGroups[groupID] ?? log.warn("StageTip Group not found GroupID : " + groupID))
// //     }
// //     getStore(ID) {
// //         if (this._TableStore == null) {
// //             this._TableStore = TableStore().Initialize('Store', GetReader('Store'))
// //         }
// //         return ID == null ?  this._TableStore : this._TableStore(ID)
// //     }
// //     getStoreCondition(ID) {
// //         if (this._TableStoreCondition == null) {
// //             this._TableStoreCondition = TableStoreCondition().Initialize('StoreCondition', GetReader('StoreCondition'))
// //         }
// //         return ID == null ?  this._TableStoreCondition : this._TableStoreCondition(ID)
// //     }
// //     getStoreConditionByGroup(groupID) {
// //         if (this.cacheStoreConditionGroups == null) {
// //             this.cacheStoreConditionGroups = {}
// //             this.getStoreCondition().Datas().forEach((key, value) => {
// //                 var array = this.cacheStoreConditionGroups[value.GroupID] ?? (this.cacheStoreConditionGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheStoreConditionGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.CheckValue - b.CheckValue })
// //             })
// //         }
// //         return groupID == null ? this.cacheStoreConditionGroups : (this.cacheStoreConditionGroups[groupID] ?? log.warn("StoreCondition Group not found GroupID : " + groupID))
// //     }
// //     getStoreCountLimit(ID) {
// //         if (this._TableStoreCountLimit == null) {
// //             this._TableStoreCountLimit = TableStoreCountLimit().Initialize('StoreCountLimit', GetReader('StoreCountLimit'))
// //         }
// //         return ID == null ?  this._TableStoreCountLimit : this._TableStoreCountLimit(ID)
// //     }
// //     getStoreCountLimitByGroup(groupID) {
// //         if (this.cacheStoreCountLimitGroups == null) {
// //             this.cacheStoreCountLimitGroups = {}
// //             this.getStoreCountLimit().Datas().forEach((key, value) => {
// //                 var array = this.cacheStoreCountLimitGroups[value.GroupID] ?? (this.cacheStoreCountLimitGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheStoreCountLimitGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.Value - b.Value })
// //             })
// //         }
// //         return groupID == null ? this.cacheStoreCountLimitGroups : (this.cacheStoreCountLimitGroups[groupID] ?? log.warn("StoreCountLimit Group not found GroupID : " + groupID))
// //     }
// //     getStorePrice(ID) {
// //         if (this._TableStorePrice == null) {
// //             this._TableStorePrice = TableStorePrice().Initialize('StorePrice', GetReader('StorePrice'))
// //         }
// //         return ID == null ?  this._TableStorePrice : this._TableStorePrice(ID)
// //     }
// //     getStorePriceByGroup(groupID) {
// //         if (this.cacheStorePriceGroups == null) {
// //             this.cacheStorePriceGroups = {}
// //             this.getStorePrice().Datas().forEach((key, value) => {
// //                 var array = this.cacheStorePriceGroups[value.StoreID] ?? (this.cacheStorePriceGroups[value.StoreID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheStorePriceGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.Count - b.Count })
// //             })
// //         }
// //         return groupID == null ? this.cacheStorePriceGroups : (this.cacheStorePriceGroups[groupID] ?? log.warn("StorePrice Group not found GroupID : " + groupID))
// //     }
// //     getSystemMessage(ID) {
// //         if (this._TableSystemMessage == null) {
// //             this._TableSystemMessage = TableSystemMessage().Initialize('SystemMessage', GetReader('SystemMessage'))
// //         }
// //         return ID == null ?  this._TableSystemMessage : this._TableSystemMessage(ID)
// //     }
// //     getSystemMessageByGroup(groupID) {
// //         if (this.cacheSystemMessageGroups == null) {
// //             this.cacheSystemMessageGroups = {}
// //             this.getSystemMessage().Datas().forEach((key, value) => {
// //                 var array = this.cacheSystemMessageGroups[value.GroupID] ?? (this.cacheSystemMessageGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheSystemMessageGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.ID - b.ID })
// //             })
// //         }
// //         return groupID == null ? this.cacheSystemMessageGroups : (this.cacheSystemMessageGroups[groupID] ?? log.warn("SystemMessage Group not found GroupID : " + groupID))
// //     }
// //     getTimedPack(ID) {
// //         if (this._TableTimedPack == null) {
// //             this._TableTimedPack = TableTimedPack().Initialize('TimedPack', GetReader('TimedPack'))
// //         }
// //         return ID == null ?  this._TableTimedPack : this._TableTimedPack(ID)
// //     }
// //     getTimedPackByGroup(groupID) {
// //         if (this.cacheTimedPackGroups == null) {
// //             this.cacheTimedPackGroups = {}
// //             this.getTimedPack().Datas().forEach((key, value) => {
// //                 var array = this.cacheTimedPackGroups[value.GroupID] ?? (this.cacheTimedPackGroups[value.GroupID] = [])
// //                 array.add(value)
// //             })
// //             this.cacheTimedPackGroups.forEach((key, value) => {
// //                 value.sort((a, b) => { return a.PayerTag - b.PayerTag })
// //             })
// //         }
// //         return groupID == null ? this.cacheTimedPackGroups : (this.cacheTimedPackGroups[groupID] ?? log.warn("TimedPack Group not found GroupID : " + groupID))
// //     }
// //     getTimeExchange(ID) {
// //         if (this._TableTimeExchange == null) {
// //             this._TableTimeExchange = TableTimeExchange().Initialize('TimeExchange', GetReader('TimeExchange'))
// //         }
// //         return ID == null ?  this._TableTimeExchange : this._TableTimeExchange(ID)
// //     }
// //     getTreasureBox(ID) {
// //         if (this._TableTreasureBox == null) {
// //             this._TableTreasureBox = TableTreasureBox().Initialize('TreasureBox', GetReader('TreasureBox'))
// //         }
// //         return ID == null ?  this._TableTreasureBox : this._TableTreasureBox(ID)
// //     }
// //     getTreasureBoxType(ID) {
// //         if (this._TableTreasureBoxType == null) {
// //             this._TableTreasureBoxType = TableTreasureBoxType().Initialize('TreasureBoxType', GetReader('TreasureBoxType'))
// //         }
// //         return ID == null ?  this._TableTreasureBoxType : this._TableTreasureBoxType(ID)
// //     }
// //     getTutorial(ID) {
// //         if (this._TableTutorial == null) {
// //             this._TableTutorial = TableTutorial().Initialize('Tutorial', GetReader('Tutorial'))
// //         }
// //         return ID == null ?  this._TableTutorial : this._TableTutorial(ID)
// //     }
// //     getUIString(ID) {
// //         if (this._TableUIString == null) {
// //             this._TableUIString = TableUIString().Initialize('UIString', GetReader('UIString'))
// //         }
// //         return ID == null ?  this._TableUIString : this._TableUIString(ID)
// //     }
// //     reload() {
// //         if (this._TableAchievement != null) { 
// //             this._TableAchievement.Initialize('Achievement', GetReader('Achievement')) 
// //         }
// //         if (this._TableAdornment != null) { 
// //             this._TableAdornment.Initialize('Adornment', GetReader('Adornment')) 
// //         }
// //         if (this._TableAnimal != null) { 
// //             this._TableAnimal.Initialize('Animal', GetReader('Animal')) 
// //         }
// //         if (this._TableAnimalRatio != null) { 
// //             this._TableAnimalRatio.Initialize('AnimalRatio', GetReader('AnimalRatio')) 
// //         }
// //         if (this._TableAnimalType != null) { 
// //             this._TableAnimalType.Initialize('AnimalType', GetReader('AnimalType')) 
// //         }
// //         if (this._TableArtifact != null) { 
// //             this._TableArtifact.Initialize('Artifact', GetReader('Artifact')) 
// //         }
// //         if (this._TableArtifactItem != null) { 
// //             this._TableArtifactItem.Initialize('ArtifactItem', GetReader('ArtifactItem')) 
// //         }
// //         if (this._TableBlocker != null) { 
// //             this._TableBlocker.Initialize('Blocker', GetReader('Blocker')) 
// //         }
// //         if (this._TableBlockerType != null) { 
// //             this._TableBlockerType.Initialize('BlockerType', GetReader('BlockerType')) 
// //         }
// //         if (this._TableBomb != null) { 
// //             this._TableBomb.Initialize('Bomb', GetReader('Bomb')) 
// //         }
// //         if (this._TableBuff != null) { 
// //             this._TableBuff.Initialize('Buff', GetReader('Buff')) 
// //         }
// //         if (this._TableBuilding != null) { 
// //             this._TableBuilding.Initialize('Building', GetReader('Building')) 
// //         }
// //         if (this._TableCommunity != null) { 
// //             this._TableCommunity.Initialize('Community', GetReader('Community')) 
// //         }
// //         if (this._TableConst != null) { 
// //             this._TableConst.Initialize('Const', GetReader('Const')) 
// //         }
// //         if (this._TableConstruction != null) { 
// //             this._TableConstruction.Initialize('Construction', GetReader('Construction')) 
// //         }
// //         if (this._TableConstructionCondition != null) { 
// //             this._TableConstructionCondition.Initialize('ConstructionCondition', GetReader('ConstructionCondition')) 
// //         }
// //         if (this._TableConstructionPhase != null) { 
// //             this._TableConstructionPhase.Initialize('ConstructionPhase', GetReader('ConstructionPhase')) 
// //         }
// //         if (this._TableCrop != null) { 
// //             this._TableCrop.Initialize('Crop', GetReader('Crop')) 
// //         }
// //         if (this._TableDealer != null) { 
// //             this._TableDealer.Initialize('Dealer', GetReader('Dealer')) 
// //         }
// //         if (this._TableDealerSlot != null) { 
// //             this._TableDealerSlot.Initialize('DealerSlot', GetReader('DealerSlot')) 
// //         }
// //         if (this._TableDecoration != null) { 
// //             this._TableDecoration.Initialize('Decoration', GetReader('Decoration')) 
// //         }
// //         if (this._TableDragon != null) { 
// //             this._TableDragon.Initialize('Dragon', GetReader('Dragon')) 
// //         }
// //         if (this._TableDragonGroup != null) { 
// //             this._TableDragonGroup.Initialize('DragonGroup', GetReader('DragonGroup')) 
// //         }
// //         if (this._TableDragonHome != null) { 
// //             this._TableDragonHome.Initialize('DragonHome', GetReader('DragonHome')) 
// //         }
// //         if (this._TableDragonTree != null) { 
// //             this._TableDragonTree.Initialize('DragonTree', GetReader('DragonTree')) 
// //         }
// //         if (this._TableDragonTreeType != null) { 
// //             this._TableDragonTreeType.Initialize('DragonTreeType', GetReader('DragonTreeType')) 
// //         }
// //         if (this._TableEnergy != null) { 
// //             this._TableEnergy.Initialize('Energy', GetReader('Energy')) 
// //         }
// //         if (this._TableEnergyExchange != null) { 
// //             this._TableEnergyExchange.Initialize('EnergyExchange', GetReader('EnergyExchange')) 
// //         }
// //         if (this._TableEntrance != null) { 
// //             this._TableEntrance.Initialize('Entrance', GetReader('Entrance')) 
// //         }
// //         if (this._TableExchangeFactory != null) { 
// //             this._TableExchangeFactory.Initialize('ExchangeFactory', GetReader('ExchangeFactory')) 
// //         }
// //         if (this._TableExchangeFactoryItem != null) { 
// //             this._TableExchangeFactoryItem.Initialize('ExchangeFactoryItem', GetReader('ExchangeFactoryItem')) 
// //         }
// //         if (this._TableExpansion != null) { 
// //             this._TableExpansion.Initialize('Expansion', GetReader('Expansion')) 
// //         }
// //         if (this._TableExpansionCondition != null) { 
// //             this._TableExpansionCondition.Initialize('ExpansionCondition', GetReader('ExpansionCondition')) 
// //         }
// //         if (this._TableExpansionCount != null) { 
// //             this._TableExpansionCount.Initialize('ExpansionCount', GetReader('ExpansionCount')) 
// //         }
// //         if (this._TableFacility != null) { 
// //             this._TableFacility.Initialize('Facility', GetReader('Facility')) 
// //         }
// //         if (this._TableFacilityList != null) { 
// //             this._TableFacilityList.Initialize('FacilityList', GetReader('FacilityList')) 
// //         }
// //         if (this._TableFacilitySlot != null) { 
// //             this._TableFacilitySlot.Initialize('FacilitySlot', GetReader('FacilitySlot')) 
// //         }
// //         if (this._TableFactory != null) { 
// //             this._TableFactory.Initialize('Factory', GetReader('Factory')) 
// //         }
// //         if (this._TableFactoryProduct != null) { 
// //             this._TableFactoryProduct.Initialize('FactoryProduct', GetReader('FactoryProduct')) 
// //         }
// //         if (this._TableFactoryProductStock != null) { 
// //             this._TableFactoryProductStock.Initialize('FactoryProductStock', GetReader('FactoryProductStock')) 
// //         }
// //         if (this._TableFactorySlotIncrease != null) { 
// //             this._TableFactorySlotIncrease.Initialize('FactorySlotIncrease', GetReader('FactorySlotIncrease')) 
// //         }
// //         if (this._TableFeatureGuide != null) { 
// //             this._TableFeatureGuide.Initialize('FeatureGuide', GetReader('FeatureGuide')) 
// //         }
// //         if (this._TableFeatureTip != null) { 
// //             this._TableFeatureTip.Initialize('FeatureTip', GetReader('FeatureTip')) 
// //         }
// //         if (this._TableField != null) { 
// //             this._TableField.Initialize('Field', GetReader('Field')) 
// //         }
// //         if (this._TableFunctionBuilding != null) { 
// //             this._TableFunctionBuilding.Initialize('FunctionBuilding', GetReader('FunctionBuilding')) 
// //         }
// //         if (this._TableIcon != null) { 
// //             this._TableIcon.Initialize('Icon', GetReader('Icon')) 
// //         }
// //         if (this._TableItem != null) { 
// //             this._TableItem.Initialize('Item', GetReader('Item')) 
// //         }
// //         if (this._Tablel10n_de != null) { 
// //             this._Tablel10n_de.Initialize('l10n_de', GetReader('l10n_de')) 
// //         }
// //         if (this._Tablel10n_en != null) { 
// //             this._Tablel10n_en.Initialize('l10n_en', GetReader('l10n_en')) 
// //         }
// //         if (this._Tablel10n_es != null) { 
// //             this._Tablel10n_es.Initialize('l10n_es', GetReader('l10n_es')) 
// //         }
// //         if (this._Tablel10n_fr != null) { 
// //             this._Tablel10n_fr.Initialize('l10n_fr', GetReader('l10n_fr')) 
// //         }
// //         if (this._Tablel10n_id != null) { 
// //             this._Tablel10n_id.Initialize('l10n_id', GetReader('l10n_id')) 
// //         }
// //         if (this._Tablel10n_it != null) { 
// //             this._Tablel10n_it.Initialize('l10n_it', GetReader('l10n_it')) 
// //         }
// //         if (this._Tablel10n_jp != null) { 
// //             this._Tablel10n_jp.Initialize('l10n_jp', GetReader('l10n_jp')) 
// //         }
// //         if (this._Tablel10n_ko != null) { 
// //             this._Tablel10n_ko.Initialize('l10n_ko', GetReader('l10n_ko')) 
// //         }
// //         if (this._Tablel10n_nl != null) { 
// //             this._Tablel10n_nl.Initialize('l10n_nl', GetReader('l10n_nl')) 
// //         }
// //         if (this._Tablel10n_pl != null) { 
// //             this._Tablel10n_pl.Initialize('l10n_pl', GetReader('l10n_pl')) 
// //         }
// //         if (this._Tablel10n_pt != null) { 
// //             this._Tablel10n_pt.Initialize('l10n_pt', GetReader('l10n_pt')) 
// //         }
// //         if (this._Tablel10n_ru != null) { 
// //             this._Tablel10n_ru.Initialize('l10n_ru', GetReader('l10n_ru')) 
// //         }
// //         if (this._Tablel10n_sc != null) { 
// //             this._Tablel10n_sc.Initialize('l10n_sc', GetReader('l10n_sc')) 
// //         }
// //         if (this._Tablel10n_tc != null) { 
// //             this._Tablel10n_tc.Initialize('l10n_tc', GetReader('l10n_tc')) 
// //         }
// //         if (this._Tablel10n_th != null) { 
// //             this._Tablel10n_th.Initialize('l10n_th', GetReader('l10n_th')) 
// //         }
// //         if (this._Tablel10n_tr != null) { 
// //             this._Tablel10n_tr.Initialize('l10n_tr', GetReader('l10n_tr')) 
// //         }
// //         if (this._TableLevel != null) { 
// //             this._TableLevel.Initialize('Level', GetReader('Level')) 
// //         }
// //         if (this._TableLoadingTip != null) { 
// //             this._TableLoadingTip.Initialize('LoadingTip', GetReader('LoadingTip')) 
// //         }
// //         if (this._TableLogin30 != null) { 
// //             this._TableLogin30.Initialize('Login30', GetReader('Login30')) 
// //         }
// //         if (this._TableLogin7New != null) { 
// //             this._TableLogin7New.Initialize('Login7New', GetReader('Login7New')) 
// //         }
// //         if (this._TableMap != null) { 
// //             this._TableMap.Initialize('Map', GetReader('Map')) 
// //         }
// //         if (this._TableMapChapter != null) { 
// //             this._TableMapChapter.Initialize('MapChapter', GetReader('MapChapter')) 
// //         }
// //         if (this._TableMapGear != null) { 
// //             this._TableMapGear.Initialize('MapGear', GetReader('MapGear')) 
// //         }
// //         if (this._TableMapGearState != null) { 
// //             this._TableMapGearState.Initialize('MapGearState', GetReader('MapGearState')) 
// //         }
// //         if (this._TableMapQuest != null) { 
// //             this._TableMapQuest.Initialize('MapQuest', GetReader('MapQuest')) 
// //         }
// //         if (this._TableMapQuestFlag != null) { 
// //             this._TableMapQuestFlag.Initialize('MapQuestFlag', GetReader('MapQuestFlag')) 
// //         }
// //         if (this._TableMonthlyCard != null) { 
// //             this._TableMonthlyCard.Initialize('MonthlyCard', GetReader('MonthlyCard')) 
// //         }
// //         if (this._TableNPC != null) { 
// //             this._TableNPC.Initialize('NPC', GetReader('NPC')) 
// //         }
// //         if (this._TableNPCTip != null) { 
// //             this._TableNPCTip.Initialize('NPCTip', GetReader('NPCTip')) 
// //         }
// //         if (this._TableOnlineGift != null) { 
// //             this._TableOnlineGift.Initialize('OnlineGift', GetReader('OnlineGift')) 
// //         }
// //         if (this._TableOrderNPC != null) { 
// //             this._TableOrderNPC.Initialize('OrderNPC', GetReader('OrderNPC')) 
// //         }
// //         if (this._TablePackage != null) { 
// //             this._TablePackage.Initialize('Package', GetReader('Package')) 
// //         }
// //         if (this._TablePayment != null) { 
// //             this._TablePayment.Initialize('Payment', GetReader('Payment')) 
// //         }
// //         if (this._TablePhaseReward != null) { 
// //             this._TablePhaseReward.Initialize('PhaseReward', GetReader('PhaseReward')) 
// //         }
// //         if (this._TablePiggyBank != null) { 
// //             this._TablePiggyBank.Initialize('PiggyBank', GetReader('PiggyBank')) 
// //         }
// //         if (this._TablePlant != null) { 
// //             this._TablePlant.Initialize('Plant', GetReader('Plant')) 
// //         }
// //         if (this._TablePlantFactory != null) { 
// //             this._TablePlantFactory.Initialize('PlantFactory', GetReader('PlantFactory')) 
// //         }
// //         if (this._TablePlantTag != null) { 
// //             this._TablePlantTag.Initialize('PlantTag', GetReader('PlantTag')) 
// //         }
// //         if (this._TablePlantTagType != null) { 
// //             this._TablePlantTagType.Initialize('PlantTagType', GetReader('PlantTagType')) 
// //         }
// //         if (this._TablePlantType != null) { 
// //             this._TablePlantType.Initialize('PlantType', GetReader('PlantType')) 
// //         }
// //         if (this._TablePoint != null) { 
// //             this._TablePoint.Initialize('Point', GetReader('Point')) 
// //         }
// //         if (this._TableProduceFactory != null) { 
// //             this._TableProduceFactory.Initialize('ProduceFactory', GetReader('ProduceFactory')) 
// //         }
// //         if (this._TablePrompt != null) { 
// //             this._TablePrompt.Initialize('Prompt', GetReader('Prompt')) 
// //         }
// //         if (this._TableQuest != null) { 
// //             this._TableQuest.Initialize('Quest', GetReader('Quest')) 
// //         }
// //         if (this._TableQuestFlag != null) { 
// //             this._TableQuestFlag.Initialize('QuestFlag', GetReader('QuestFlag')) 
// //         }
// //         if (this._TableSlots != null) { 
// //             this._TableSlots.Initialize('Slots', GetReader('Slots')) 
// //         }
// //         if (this._TableSlotsIcon != null) { 
// //             this._TableSlotsIcon.Initialize('SlotsIcon', GetReader('SlotsIcon')) 
// //         }
// //         if (this._TableSlotsReward != null) { 
// //             this._TableSlotsReward.Initialize('SlotsReward', GetReader('SlotsReward')) 
// //         }
// //         if (this._TableSpecialPrompt != null) { 
// //             this._TableSpecialPrompt.Initialize('SpecialPrompt', GetReader('SpecialPrompt')) 
// //         }
// //         if (this._TableStage != null) { 
// //             this._TableStage.Initialize('Stage', GetReader('Stage')) 
// //         }
// //         if (this._TableStageTip != null) { 
// //             this._TableStageTip.Initialize('StageTip', GetReader('StageTip')) 
// //         }
// //         if (this._TableStore != null) { 
// //             this._TableStore.Initialize('Store', GetReader('Store')) 
// //         }
// //         if (this._TableStoreCondition != null) { 
// //             this._TableStoreCondition.Initialize('StoreCondition', GetReader('StoreCondition')) 
// //         }
// //         if (this._TableStoreCountLimit != null) { 
// //             this._TableStoreCountLimit.Initialize('StoreCountLimit', GetReader('StoreCountLimit')) 
// //         }
// //         if (this._TableStorePrice != null) { 
// //             this._TableStorePrice.Initialize('StorePrice', GetReader('StorePrice')) 
// //         }
// //         if (this._TableSystemMessage != null) { 
// //             this._TableSystemMessage.Initialize('SystemMessage', GetReader('SystemMessage')) 
// //         }
// //         if (this._TableTimedPack != null) { 
// //             this._TableTimedPack.Initialize('TimedPack', GetReader('TimedPack')) 
// //         }
// //         if (this._TableTimeExchange != null) { 
// //             this._TableTimeExchange.Initialize('TimeExchange', GetReader('TimeExchange')) 
// //         }
// //         if (this._TableTreasureBox != null) { 
// //             this._TableTreasureBox.Initialize('TreasureBox', GetReader('TreasureBox')) 
// //         }
// //         if (this._TableTreasureBoxType != null) { 
// //             this._TableTreasureBoxType.Initialize('TreasureBoxType', GetReader('TreasureBoxType')) 
// //         }
// //         if (this._TableTutorial != null) { 
// //             this._TableTutorial.Initialize('Tutorial', GetReader('Tutorial')) 
// //         }
// //         if (this._TableUIString != null) { 
// //             this._TableUIString.Initialize('UIString', GetReader('UIString')) 
// //         }
// //     }
// // }
// // class Test {
// //     constructor() {
// //         print("0000000 : " + this)
// //     }
// // }
// // class Test1 : Test {
// //     constructor() {
// //         print(base)
// //         base.constructor()
// //         this.a = 100
// //         print("11111 : " + this)
// //     }
// //     hello() {
// //         print("hello1")
// //     }
// // }
// // class Test2 : Test1 {
// //     constructor() {
// //         print(base)
// //         base.constructor()
// //         this.b = 200
// //         print("22222 : " + this)
// //         print(this.a, this.b)
// //     }
// //     hello() {
// //         print("hello2")
// //     }
// // }
// // class Test3 : Test1 {
// //     constructor() {
// //         this.a = 500
// //         base.constructor()
// //         this.b = 300
// //         print("33333 : " + this)
// //         print(this.a, this.b)
// //         print(base.a)
// //     }
// //     hello() {
// //         print("hello3")
// //     }
// // }
// // var t2 = new Test2()
// // // var t3 = new Test3()
// // // t2.hello()
// // // t3.hello()
// // print(getPrototype(getPrototype(t2)))
// // // aaa = {}
// // // b = 100
// // // print(aaa, b)
// // // var a = aaa[b] ?? (aaa[b] = [])
// // // a.add(100)
// // // var a = createStringBuilder()
// // // for (var i = 0, 100) {
// // //     a.append('0')
// // // }
// // // a.setLength(100)
// // // a[10]= 97
// // // print(a)
// // // a[50] = 98
// // // print(a)
// // // a = []
// // // (b ?? a).add(100)
// // // print(a)

// // // var TestStruct = import_type("ScorpioExec.TestStruct")
// // // var t = TestStruct()
// // // t.value1 = 200
// // // t.value2 = 300
// // // t.staticNumber = 500
// // // TestStruct.staticNumber = 1000
// // // print(t.value1, t.value2, t.staticNumber)
// // // var a = [100,200]
// // // foreach (var pair in pairs(a)) {
// // //     try {
// // //         b()
// // //     } catch (e) {
// // //         print(pair)
// // //     }
// // // }
// // // tab = {
// // //     test() {
// // //         this.testfun?.()
// // //         print("fewafawefawefaewf")
// // //     }
// // //     testfun() {
// // //         print("tetwt")
// // //     }
// // // }
// // // tab.test()
// // // function test() {
// // //     try {
// // //         throw "fewafwaef"
// // //     } catch (e) {
// // //         print("fewafawefwea")
// // //     }
// // // }
// // // test()
// // // CSharpSingle = {
// // //     PositiveInfinity : 1
// // // }
// // // class Color {
// // // }
// // // class Vector2 {

// // // }
// // // Screen = {
// // //     width = 1,
// // //     height = 1,
// // // }
// // // LayerMask = {
// // //     NameToLayer() {
// // //         return 1
// // //     }
// // // }
// // // FileUtil = {
// // //     GetMD5FromString() {
// // //         return "123123"
// // //     }
// // // }

// // // CONFIG_CUSTOM_ACCOUNT = "CONFIG_CUSTOM_ACCOUNT"
// // // CONFIG_CUSTOM_ENTRY_URL = "CONFIG_CUSTOM_ENTRY_URL"

// // // SinglePositiveInfinity = CSharpSingle.PositiveInfinity      //float 正无穷

// // // ColorRed = Color(1, 0, 0, 1)        //红色
// // // ColorWhite = Color.white            //白色
// // // ColorGreen = Color(0, 1, 0, 1)      //绿色
// // // ColorGray = Color(0.65, 0.65, 0.65) //灰色

// // // TileEdgeColor = Color(1, 1, 1, 0.5)             //格子在云边缘颜色
// // // TileNormalColor = Color.white                   //正常格子颜色
// // // TileActiveColor = Color(0.7, 0.7, 0.7, 1)       //格子被选中后闪烁颜色
// // // TileFieldActiveClour = Color(0.7, 0.7, 0.7, 1)  //田地被选择后的颜色

// // // ScreenWidth = Screen.width                           //屏幕宽度
// // // ScreenHeight = Screen.height                         //屏幕高度
// // // ScreenCenter = new Vector2(ScreenWidth / 2, ScreenHeight / 2)   //屏幕中心点

// // // UIHeight = 750                                       //UI虚拟屏幕高度
// // // UIWidth = ScreenWidth / ScreenHeight * UIHeight      //UI虚拟屏幕宽度
// // // UIVirtualHeight = 750                                //UI虚拟高度
// // // UIVirtualWidth = 1334                                //UI虚拟宽度

// // // LayerTerrain = 1L << LayerMask.NameToLayer("Terrain")       // Terrain 层

// // // ITEM_DIAMOND = 100000       //钻石ID
// // // ITEM_EXP = 101111           //经验
// // // ITEM_COIN = 102222          //金币
// // // ITEM_HEART = 103333         //爱心
// // // ITEM_ENERGY = 104444        //体力

// // // STORE_TAB_DIAMOND = 0       //商店 钻石 tab页
// // // STORE_TAB_HEART = 1         //商店 爱心 tab页
// // // STORE_TAB_COIN = 2          //商店 金币 tab页
// // // STORE_TAB_ENERGY = 3        //商店 体力 tab页

// // // SHOP_TAB_FACTORY = 0        //商店 工厂 tab页
// // // SHOP_TAB_BUILDING = 1       //商店 建筑 tab页
// // // SHOP_TAB_CROP = 2           //商店 作物 tab页
// // // SHOP_TAB_DECORATION = 3     //商店 装饰 tab页
// // // SHOP_TAB_RECOVERED = 4      //商店 回收 tab页

// // //GM 密钥
// // // GM_SECRET_KEY = FileUtil.GetMD5FromString("${Application.identifier}_${Application.platform}_${Application.version}_${SystemInfo.deviceModel}_${SystemInfo.deviceName}_${SystemInfo.systemMemorySize}_${math.floor(io.unixNow() / 1000 / 86400)}")

// // // PAUSE_DRAGON_AI = false

// // // CloudClickEffectID = 3      //点击云雾特效ID
// // // HillClickEffectID = 4       //点击石头特效ID


// // // bbb()
// // // function ccc(a,b,c) {
// // //     print(a,b,c)
// // // }
// // // ccc?.(aaa()...)
// // // print("over")
// // // print(io.unixNow() / 1000 / 86400)
// // // print(io.unixNow())
// // // TestClass = import_type("ScorpioExec.TestClass")
// // // function test() {
// // //     TestClass.TestStaticFunc("test1")
// // // }
// // // function test1() {
// // //     TestClass.TestStaticFunc("test2")
// // // }
// // // function test2() {
// // //     throw "fewafwaefwaf"
// // // }
// // // test()
// // // function test() {
// // //     throw "12312312312312213"
// // // }
// // // function test1() {
// // //     test()
// // // }
// // // test1()
// // // function test() {
// // //     try {
// // //         print("try1")
// // //         try {
// // //             print("try2")
// // //             throw "fewafwaefwa"
// // //         } catch (e) {
// // //             print("catch2 : " + e)
// // //         }
// // //         print("12312312")
// // //     } catch (e) {
// // //         print("catch1")
// // //     }
// // // }
// // // test()
// // // print("hello")

// // // function www() {

// // // }

// // // var tab = {}
// // // var tab1 = {}
// // // var a = print.bind(tab)
// // // var b = print.bind(tab)
// // // a("111111111")

// // // var arr = []
// // // arr.add(a)
// // // print(arr.length())
// // // arr.remove(print.bind(tab))
// // // print(arr.length())
// // // TestClass = import_type("ScorpioExec.TestClass")
// // // var t = new TestClass(111)
// // // function aaa() {
// // //     t.num = "1232131"
// // //     // TestClass.TestStaticFunc()
// // // }
// // // function bbb() {
// // //     aaa()
// // // }
// // // function ccc() {
// // //     bbb()
// // // }
// // // ccc()



// // // importExtension("ScorpioExec.TestClassEx")
// // // TestClass = import_type("ScorpioExec.TestClass")
// // // var a = TestClass(100);
// // // a.TestFuncEx(500)
// // // print(a.TestNumber)
// // // StoreTabType = {
// // //     Coin : 2,
// // //     Heart : 3,
// // //     Energy : 4,
// // //     Dragon : 21,
// // //     Building : 22,
// // //     Crop : 23,
// // //     Factory : 25,
// // //     Decoration : 40,
// // //     GetString : function(id) {
// // //         switch (id) {
// // //             case 2: return 'Coin'; 
// // //             case 3: return 'Heart'; 
// // //             case 4: return 'Energy'; 
// // //             case 21: return 'Dragon'; 
// // //             case 22: return 'Building'; 
// // //             case 23: return 'Crop'; 
// // //             case 25: return 'Factory'; 
// // //             case 40: return 'Decoration'; 
// // //         }
// // //     }
// // // }
// // // ReddotType = {

// // // }
// // // function hello(a) {
// // //     switch (a) {
// // //         case 0: return;
// // //         case 1: return;
// // //     }
// // // }
// // // hello(0)

// // // var a = "AAA"
// // // print(a.toOneLower())

// // // DownloadStatus = {
// // //     RequestAssets : 100
// // // }
// // // class DownloadAssets {
// // //     SetStatus(status) {
// // //         print("setstatus : ", this)
// // //     }
// // //     Exe() {
// // //         this.SetStatus(DownloadStatus.RequestAssets)
// // //     }
// // // }
// // // var d = DownloadAssets()
// // // d.Exe()
// // // b = {a = "222"}
// // // var a = b?.[test()]
// // // print(a)
// // // var a = false
// // // a |= true
// // // print(a)
// // // print(a)
// // // TestClass = import_type("ScorpioExec.TestClass")
// // // importExtension("ScorpioExec.TestClassEx")
// // // importExtension("ScorpioExec.TestClassEx")
// // // var a = TestClass(200)
// // // // a.TestFuncEx(1111, 2222, 1, 2, 3)
// // // var ref = {value = 11111}
// // // a.TestFuncEx(ref)
// // // print(ref)
// // // a.TestArgs(1111, 1, 2, 3)
// // // class Cl {

// // // }
// // // var a = Cl()
// // // for (var i = 0 ; i < 10000000; i++) {
// // // 	a.a = 100
// // // 	a.b = 100
// // // 	a.c = 200
// // // 	a.d = 2222
// // // 	a.e = "feawfaewf"
// // // 	a.f = "wefwae"
// // // 	a.h = "wweee"
// // // }
// // // var a = null
// // // var b = false
// // // var c = (a ?? b) ?? "fewa"
// // // print(c)
// // //print("hello world")
// // //print(String.format("aaaa{}wwww", 100, 200))
// // //var d = !w
// // //print (d)
// // // var a = json.decode(`{"aaaa" : 100L}`)
// // // print(a)
// // // var a = []
// // // a.addUnique(100)
// // // a.addUnique(100)
// // // print(a.popLast())
// // // print(a.popLast())
// // // function hello(a1,a2,a3,a4) {
// // // }
// // // for (var i = 0 ; i < 1000000; i ++) {
// // //     hello([100,200]...,[300,400]...)
// // // }
// // // for (var i = 0 ; i < 1000000; i ++) {
// // //     hello([100,200],[300,400])
// // // }
// // // hello([100,200]...,[300,400], ["feawf", "feawfafw"]...)
// // // // var a = b == 100 ? 100 : 200
// // // // print(a)
// // // UI_DOWNLOAD = {

// // // }
// // // UI = {
// // //     Objects = {},       		//所有UI的信息
// // //     function InitializeUI(index) {
// // //         // print("=================== " + index)
// // // 		// if (this.Objects.containsKey(index)) { return this.Objects[index]; }
// // // 		// print("22222222222222222222222222222 " + index)
// // //         // var str = "${index}_ATTR"
// // //         // print(str)
// // //         // var attribute = _G["${index}_ATTR"]
// // //         var com = UI_DOWNLOAD
// // //         // var com = isMap(value) ? clone(value) : value()		//如果是map就clone 否则就是 class 直接new一个对象
// // //         // var com = clone(value)
// // //         com.Hide = function(args) { UI.Hide(index, args) }
// // //         com.Hide()
// // //     }
// // // }
// // // UI.InitializeUI("feawfaewfaewf")
// // // // class www {

// // // // }
// // // // try {
// // // //     throw "1111"
// // // //     throw new www()
// // // // } catch (e) {
// // // //     if (getPrototype(e) == www) {

// // // //     }
// // // // }
// // // // var a = 100
// // // // var b = 111
// // // // switch (a) {
// // // //     case 50 + 50:
// // // //     case 300:
// // // //         for (var i = 0;i<100;i++) {
// // // //             if (i == 10) {
// // // //                 print("ffffffffffffff")
// // // //                 break
// // // //             }
// // // //         }
// // // //         print("111111111111")
// // // //     case 200:
// // // //         print(200)
        
// // // //     default:
// // // //         print("default")
// // // //         break
// // // // }

// // // // class Cl {

// // // // }
// // // // var a = new Cl()
// // // // setPropertys(a, {a : 100, b : 200})
// // // // print(json.encode(a))
// // // // var t = {
// // // //     function Test() {
// // // //         return t
// // // //     }
// // // // }   

// // // // t.Test() { a : 100}
// // // // var a = t.Test() { b : 200}
// // // // print(t)
// // // // Test().testFunc()
// // // // function test() {

// // // // }
// // // // var a = [100,200]
// // // // var b = [200,300]
// // // // test(a..., vvv, b... )
// // // // var eee = 0
// // // // for (var i = 0; i < 10000000; i += 1) {
// // // //     var a = i + 1
// // // //     var b = 2.3
// // // //     if(a < b) {
// // // //         a = a + 1
// // // //     } else {
// // // //         b = b + 1
// // // //     }
// // // //     if(a == b){
// // // //         b = b + 1
// // // //     }
// // // //     eee = eee + a * b + a / b
// // // // }
// // // // print(eee)
// // // // Math = import_type("System.Math")
// // // // for (var i = 0, 10000000) {
// // // //     Math.Min(10,20)
// // // // }
// // // // var a = 100L
// // // // var b = 100
// // // // print(isDouble(b + a))
// // // // var a = null
// // // // var a = {
// // // //     num = 100,
// // // //     func() {
// // // //         var b = () => {
// // // //             print(this)
// // // //         }
// // // //         b()
// // // //     }
// // // // }
// // // // a.func()