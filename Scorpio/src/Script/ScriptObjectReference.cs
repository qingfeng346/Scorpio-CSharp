#define SCRIPT_OBJECT
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
        public int objectEntityLength = 0;
        public Entity<EntityValue>[] objectEntities = new Entity<EntityValue>[Stage];

        public int objectPoolLength = 0;
        public int[] objectPool = new int[Stage];

        public int objectFreeLength = 0;
        public int[] objectFrees = new int[Stage];
        public int Alloc(EntityValue value) {
#if SCRIPT_OBJECT
            if (value.Index > 0) {
                ++objectEntities[value.Index].referenceCount;
                return value.Index;
            }
            int index;
#else
            if (string2Index.TryGetValue(value, out var index)) {
                ++objectEntities[index].referenceCount;
                return index;
            }
#endif
            if (objectPoolLength > 0) {
                index = objectPool[--objectPoolLength];
            } else {
                if (objectEntityLength == objectEntities.Length) {
                    var newEntities = new Entity<EntityValue>[objectEntityLength + Stage];
                    Array.Copy(objectEntities, newEntities, objectEntityLength);
                    objectEntities = newEntities;
                }
                index = objectEntityLength++;
#if SCORPIO_DEBUG
                objectEntities[index].index = index;
#endif
            }
#if SCRIPT_OBJECT
            value.Index = index;
#else
            string2Index.Add(value, index);
#endif
            objectEntities[index].value = value;
            objectEntities[index].referenceCount = 1;
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
            if (objectPoolLength > 0) {
                index = objectPool[--objectPoolLength];
            } else {
                if (objectEntityLength == objectEntities.Length) {
                    var newEntities = new Entity<EntityValue>[objectEntityLength + Stage];
                    Array.Copy(objectEntities, newEntities, objectEntityLength);
                    objectEntities = newEntities;
                }
                index = objectEntityLength++;
#if SCORPIO_DEBUG
                objectEntities[index].index = index;
#endif
            }
#if SCRIPT_OBJECT
            value.Index = index;
#else
            string2Index.Add(value, index);
#endif
            objectEntities[index].value = value;
            objectEntities[index].referenceCount = 0;
            if (objectFreeLength == objectFrees.Length) {
                var newFrees = new int[objectFreeLength + Stage];
                Array.Copy(objectFrees, newFrees, objectFreeLength);
                objectFrees = newFrees;
            }
            objectFrees[objectFreeLength++] = index;
            return index;
        }
#if SCRIPT_OBJECT
        public int GetObjectReferenceCount(int index) {
#else
        public int GetStringReferenceCount(int index) {
#endif
            return objectEntities[index].referenceCount;
        }
#if SCRIPT_OBJECT
        public void FreeObject(int index) {
#else
        public void FreeString(int index) {
#endif
            if ((--objectEntities[index].referenceCount) == 0) {
                //添加到待释放列表
                if (objectFreeLength == objectFrees.Length) {
                    var newFrees = new int[objectFreeLength + Stage];
                    Array.Copy(objectFrees, newFrees, objectFreeLength);
                    objectFrees = newFrees;
                }
                objectFrees[objectFreeLength++] = index;
            }
#if SCORPIO_DEBUG
            if (objectEntities[index].referenceCount < 0) {
                ScorpioLogger.error($"{typeof(EntityValue)} 释放有问题,当前计数:{objectEntities[index].referenceCount}  Index:{index} - {objectEntities[index]}");
            }
#endif
        }
#if SCRIPT_OBJECT
        public void ReferenceObject(int index) {
#else
        public void ReferenceString(int index) {
#endif
            ++objectEntities[index].referenceCount;
        }
#if SCRIPT_OBJECT
        public EntityValue GetObjectValue(int index) {
#else
        public EntityValue GetStringValue(int index) {
#endif
            return objectEntities[index].value;
        }
        //释放
#if SCRIPT_OBJECT
        public unsafe void ReleaseAllObject() {
#else
        public unsafe void ReleaseAllString() {
#endif
            if (objectFreeLength > 0) {
                int index;
                Entity<EntityValue> entity;
                for (var i = 0; i < objectFreeLength; ++i) {
                    index = objectFrees[i];
                    entity = objectEntities[index];
                    if (entity.referenceCount == 0) {
#if SCRIPT_OBJECT
                        entity.value.Index = -1;
                        entity.value.Free();
#else
                        string2Index.Remove(entity.value);
#endif
                        if (objectPoolLength == objectPool.Length) {
                            var newPool = new int[objectPoolLength + Stage];
                            Array.Copy(objectPool, newPool, objectPoolLength);
                            objectPool = newPool;
                        }
                        objectPool[objectPoolLength++] = index;
                        objectEntities[index].value = null;
                        objectEntities[index].referenceCount = -1;
                    }
                }
                objectFreeLength = 0;
            }
        }
        //检查未释放
#if SCRIPT_OBJECT
        public unsafe void CheckObject() {
#else
        public unsafe void CheckString() {
#endif
            for (var i = 0; i < objectEntityLength; ++i) {
                if (objectEntities[i].value != null) {
                    ScorpioLogger.error($"当前未释放{typeof(EntityValue)}变量 索引:{i}  {objectEntities[i]}");
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
            Array.Clear(objectEntities, 0, objectEntityLength);
            objectEntityLength = 0;
            objectPoolLength = 0;
            objectFreeLength = 0;
        }
    }
}
