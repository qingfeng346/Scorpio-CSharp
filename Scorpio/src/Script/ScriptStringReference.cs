using System;
using System.Collections.Generic;
using Scorpio.Tools;
#if SCRIPT_OBJECT
using EntityValue = Scorpio.ScriptObject;
#else
using EntityValue = System.String;
#endif
namespace Scorpio {
    public partial class Script {
#if !SCRIPT_OBJECT
        public Dictionary<string, int> string2Index = new Dictionary<string, int>();
#endif
        public int stringEntityLength = 0;
        public Entity<EntityValue>[] stringEntities = new Entity<EntityValue>[Stage];

        public int stringPoolLength = 0;
        public int[] stringPool = new int[Stage];

        public int stringFreeLength = 0;
        public int[] stringFrees = new int[Stage];
        public int Alloc(EntityValue value) {
#if SCRIPT_OBJECT
            if (value.Index > 0) {
                ++objectEntities[value.Index].referenceCount;
                return value.Index;
            }
            int index;
#else
            if (string2Index.TryGetValue(value, out var index)) {
                ++stringEntities[index].referenceCount;
                return index;
            }
#endif
            if (stringPoolLength > 0) {
                index = stringPool[--stringPoolLength];
            } else {
                if (stringEntityLength == stringEntities.Length) {
                    var newEntities = new Entity<EntityValue>[stringEntityLength + Stage];
                    Array.Copy(stringEntities, newEntities, stringEntityLength);
                    stringEntities = newEntities;
                }
                index = stringEntityLength++;
#if SCORPIO_DEBUG
                stringEntities[index].index = index;
#endif
            }
#if SCRIPT_OBJECT
            value.Index = index;
#else
            string2Index.Add(value, index);
#endif
            stringEntities[index].value = value;
            stringEntities[index].referenceCount = 1;
            return index;
        }
        //获取index,如果是新创建的立刻加入释放列表
        public int GetIndex(EntityValue value) {
#if SCRIPT_OBJECT
            if (value.Index > 0) {
                return value.Index;
            }
            int index;
#else
            if (string2Index.TryGetValue(value, out var index)) {
                return index;
            }
#endif
            if (stringPoolLength > 0) {
                index = stringPool[--stringPoolLength];
            } else {
                if (stringEntityLength == stringEntities.Length) {
                    var newEntities = new Entity<EntityValue>[stringEntityLength + Stage];
                    Array.Copy(stringEntities, newEntities, stringEntityLength);
                    stringEntities = newEntities;
                }
                index = stringEntityLength++;
#if SCORPIO_DEBUG
                stringEntities[index].index = index;
#endif
            }
#if SCRIPT_OBJECT
            value.Index = index;
#else
            string2Index.Add(value, index);
#endif
            stringEntities[index].value = value;
            stringEntities[index].referenceCount = 0;
            if (stringFreeLength == stringFrees.Length) {
                var newFrees = new int[stringFreeLength + Stage];
                Array.Copy(stringFrees, newFrees, stringFreeLength);
                stringFrees = newFrees;
            }
            stringFrees[stringFreeLength++] = index;
            return index;
        }
#if SCRIPT_OBJECT
        public int GetObjectReferenceCount(int index) {
#else
        public int GetStringReferenceCount(int index) {
#endif
            return stringEntities[index].referenceCount;
        }
#if SCRIPT_OBJECT
        public void FreeObject(int index) {
#else
        public void FreeString(int index) {
#endif
            if ((--stringEntities[index].referenceCount) == 0) {
                //添加到待释放列表
                if (stringFreeLength == stringFrees.Length) {
                    var newFrees = new int[stringFreeLength + Stage];
                    Array.Copy(stringFrees, newFrees, stringFreeLength);
                    stringFrees = newFrees;
                }
                stringFrees[stringFreeLength++] = index;
            }
#if SCORPIO_DEBUG
            if (stringEntities[index].referenceCount < 0) {
                ScorpioLogger.error($"{typeof(EntityValue)} 释放有问题,当前计数:{stringEntities[index].referenceCount}  Index:{index} - {stringEntities[index]}");
            }
#endif
        }
#if SCRIPT_OBJECT
        public void ReferenceObject(int index) {
#else
        public void ReferenceString(int index) {
#endif
            ++stringEntities[index].referenceCount;
        }
#if SCRIPT_OBJECT
        public EntityValue GetObjectValue(int index) {
#else
        public EntityValue GetStringValue(int index) {
#endif
            return stringEntities[index].value;
        }
        //释放
#if SCRIPT_OBJECT
        public unsafe void ReleaseAllObject() {
#else
        public unsafe void ReleaseAllString() {
#endif
            if (stringFreeLength > 0) {
                int index;
                Entity<EntityValue> entity;
                for (var i = 0; i < stringFreeLength; ++i) {
                    index = stringFrees[i];
                    entity = stringEntities[index];
                    if (entity.referenceCount == 0) {
#if SCRIPT_OBJECT
                        entity.value.Index = -1;
                        entity.value.Free();
#else
                        string2Index.Remove(entity.value);
#endif
                        if (stringPoolLength == stringPool.Length) {
                            var newPool = new int[stringPoolLength + Stage];
                            Array.Copy(stringPool, newPool, stringPoolLength);
                            stringPool = newPool;
                        }
                        stringPool[stringPoolLength++] = index;
                        stringEntities[index].value = null;
                        stringEntities[index].referenceCount = -1;
                    }
                }
                stringFreeLength = 0;
            }
        }
        //检查未释放
#if SCRIPT_OBJECT
        public unsafe void CheckObject() {
#else
        public unsafe void CheckString() {
#endif
            for (var i = 0; i < stringEntityLength; ++i) {
                if (stringEntities[i].value != null) {
                    ScorpioLogger.error($"当前未释放{typeof(EntityValue)}变量 索引:{i}  {stringEntities[i]}");
                }
            }
        }
#if SCRIPT_OBJECT
        public void ShutdownObject() {
#else
        public void ShutdownString() {
#endif
#if !SCRIPT_OBJECT
            string2Index.Clear();
#endif
            Array.Clear(stringEntities, 0, stringEntityLength);
            stringEntityLength = 0;
            stringPoolLength = 0;
            stringFreeLength = 0;
        }
    }
}
