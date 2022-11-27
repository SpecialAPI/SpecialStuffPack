using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using SpecialStuffPack.SaveAPI;

namespace SpecialStuffPack
{
    public static class SpriteBuilder
    {
        public static tk2dSpriteCollectionData itemCollection = PickupObjectDatabase.GetById(155).sprite.Collection;
        public static tk2dSpriteCollectionData ammonomiconCollection = AmmonomiconController.ForceInstance.EncounterIconCollection;
        public static Dictionary<Texture2D, Vector2> TextureTrimOffsets = new();
        public static Dictionary<Texture2D, Vector2> TextureTrimUntrimmedCenters = new();
        public static Dictionary<Texture2D, Vector2> TextureTrimUntrimmedExtents = new();

        public static SpeculativeRigidbody SetUpSpeculativeRigidbody(this tk2dSprite sprite, IntVector2 offset, IntVector2 dimensions)
        {
            SpeculativeRigidbody orAddComponent = sprite.gameObject.GetOrAddComponent<SpeculativeRigidbody>();
            PixelCollider pixelCollider = new PixelCollider()
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.EnemyCollider,
                ManualWidth = dimensions.x,
                ManualHeight = dimensions.y,
                ManualOffsetX = offset.x,
                ManualOffsetY = offset.y
            };
            orAddComponent.PixelColliders = new List<PixelCollider>
            {
                pixelCollider
            };
            return orAddComponent;
        }

        /// <summary>
        /// Returns an object with a tk2dSprite component with the 
        /// texture of an embedded resource
        /// </summary>
        public static GameObject SpriteFromResource(string spriteName, string shaderName, GameObject obj = null)
        {
            var texture = AssetBundleManager.specialeverything.LoadAsset<Texture2D>(spriteName);
            if (texture == null) return null;

            return SpriteFromTexture(spriteName, shaderName, obj);
        }

        /// <summary>
        /// Returns an object with a tk2dSprite component with the texture provided
        /// </summary>
        public static GameObject SpriteFromTexture(string spriteName, string shaderName, GameObject obj = null)
        {
            if (obj == null)
            {
                obj = new GameObject();
            }
            tk2dSprite sprite;
            sprite = obj.AddComponent<tk2dSprite>();

            int id = AddSpriteToCollection(spriteName, itemCollection, shaderName);
            sprite.SetSprite(itemCollection, id);
            sprite.SortingOrder = 0;

            obj.GetComponent<BraveBehaviour>().sprite = sprite;
            obj.SetActive(false);

            return obj;
        }

        public static void ResetOffset(this tk2dSpriteDefinition def)
        {
            def.AddOffset(-def.position0);
        }
        
        public static void ApplyOffsetsToAnimation(this tk2dSpriteAnimationClip clip, List<IntVector2> offsets)
        {
            ApplyOffsetsToAnimation(clip, offsets.ConvertAll((IntVector2 vec) => new Vector2(vec.x / 16f, vec.y / 16f)));
        }

        public static void ApplyOffsetsToAnimation(this tk2dSpriteAnimationClip clip, List<Vector2> offsets)
        {
            if(clip != null && clip.frames != null && offsets != null)
            {
                int shortest = clip.frames.Length < offsets.Count ? clip.frames.Length : offsets.Count;
                for (int i = 0; i < shortest; i++)
                {
                    if (clip.frames[i] != null && clip.frames[i].spriteCollection != null && clip.frames[i].spriteCollection.spriteDefinitions != null && clip.frames[i].spriteId >= 0 && 
                        clip.frames[i].spriteCollection.spriteDefinitions[clip.frames[i].spriteId] != null)
                    {
                        clip.frames[i].spriteCollection.spriteDefinitions[clip.frames[i].spriteId].AddOffset(offsets[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a sprite (from a resource) to a collection
        /// </summary>
        /// <returns>The spriteID of the defintion in the collection</returns>
        public static int AddSpriteToCollection(string resourcePath, tk2dSpriteCollectionData collection, string shaderName, bool noDuplicates = false, bool disableTrimming = false)
        {
            if(collection == null)
            {
                Debug.LogWarning("Collection is null! Resource path: " + resourcePath);
                return -1;
            }
            var texture = AssetBundleManager.Load<Texture2D>(resourcePath); //Get Texture
            if(texture == null)
            {
                Debug.LogWarning("Texture is null! Resource path: " + resourcePath);
                return -1;
            }
            else if(noDuplicates)
            {
                var id = collection.GetSpriteIdByName(texture.name, -1);
                if(id >= 0)
                {
                    return id;
                }
            }
            return AddSpriteToCollection(texture, collection, shaderName, noDuplicates, disableTrimming);
        }

        /// <summary>
        /// Adds a sprite (from a resource) to a collection
        /// </summary>
        /// <returns>The spriteID of the defintion in the collection</returns>
        public static int AddSpriteToCollection(Texture2D texture, tk2dSpriteCollectionData collection, string shaderName, bool noDuplicates = false, bool disableTrimming = false)
        {
            if (collection == null)
            {
                Debug.LogWarning("Collection is null!");
                return -1;
            }
            if (texture == null)
            {
                Debug.LogWarning("Texture is null!");
                return -1;
            }
            else if (noDuplicates)
            {
                var id = collection.GetSpriteIdByName(texture.name, -1);
                if (id >= 0)
                {
                    return id;
                }
            }
            if (TextureTrimOffsets.TryGetValue(texture, out var offset))
            {
                TextureTrimUntrimmedCenters.TryGetValue(texture, out var untrimmedCenter);
                TextureTrimUntrimmedExtents.TryGetValue(texture, out var untrimmedExtents);
                var definition = ConstructDefinition(texture, shaderName, untrimmedCenter, untrimmedExtents); //Generate definition
                definition.AddOffset(offset);
                definition.name = texture.name; //naming the definition is actually extremely important
                return AddSpriteToCollection(definition, collection);
            }
            else if(!disableTrimming)
            {
                Vector3 untrimmedCenter = new(texture.width / 16f / 2f, texture.height / 16f / 2f, 0f);
                Vector3 untrimmedExtents = new(texture.width / 16f, texture.height / 16f, 0f);
                var pixelOffset = texture.TrimTexture();
                var definition = ConstructDefinition(texture, shaderName, untrimmedCenter, untrimmedExtents); //Generate definition
                var unitOffset = pixelOffset.ToVector2() / 16f;
                definition.AddOffset(unitOffset);
                definition.name = texture.name; //naming the definition is actually extremely important
                TextureTrimOffsets.Add(texture, unitOffset);
                TextureTrimUntrimmedCenters.Add(texture, untrimmedCenter);
                TextureTrimUntrimmedExtents.Add(texture, untrimmedExtents);

                return AddSpriteToCollection(definition, collection);
            }
            else
            {
                var definition = ConstructDefinition(texture, shaderName);
                definition.name = texture.name;
                return AddSpriteToCollection(definition, collection);
            }
        }

        public static void TrimGunSprites(this Gun gun)
        {
            List<KeyValuePair<tk2dSpriteCollectionData, int>> ids = new();
            gun.TryTrimGunAnimation(gun.shootAnimation, ids);
            gun.TryTrimGunAnimation(gun.reloadAnimation, ids);
            gun.TryTrimGunAnimation(gun.emptyReloadAnimation, ids);
            gun.TryTrimGunAnimation(gun.idleAnimation, ids);
            gun.TryTrimGunAnimation(gun.chargeAnimation, ids);
            gun.TryTrimGunAnimation(gun.dischargeAnimation, ids);
            gun.TryTrimGunAnimation(gun.emptyAnimation, ids);
            gun.TryTrimGunAnimation(gun.introAnimation, ids);
            gun.TryTrimGunAnimation(gun.finalShootAnimation, ids);
            gun.TryTrimGunAnimation(gun.enemyPreFireAnimation, ids);
            gun.TryTrimGunAnimation(gun.outOfAmmoAnimation, ids);
            gun.TryTrimGunAnimation(gun.criticalFireAnimation, ids);
            gun.TryTrimGunAnimation(gun.dodgeAnimation, ids);
            var defaultId = gun.sprite.spriteId;
            var defaultDefinition = gun.sprite.Collection.spriteDefinitions[defaultId];
            var globalOffset = new Vector2(-defaultDefinition.position0.x, -defaultDefinition.position0.y);
            foreach (var x in ids)
            {
                x.Key?.spriteDefinitions[x.Value]?.AddOffset(globalOffset);
                var attach = x.Key?.GetAttachPoints(x.Value);
                if (attach == null)
                {
                    continue;
                }
                foreach (var attachPoint in attach)
                {
                    attachPoint.position += globalOffset.ToVector3ZUp(0f);
                }
            };
        }

        public static void TryTrimGunAnimation(this Gun gun, string animation, List<KeyValuePair<tk2dSpriteCollectionData, int>> ids)
        {
            if (!string.IsNullOrEmpty(animation) && gun.spriteAnimator != null)
            {
                var clip = gun.spriteAnimator.GetClipByName(animation);
                if(clip != null)
                {
                    foreach(var frame in clip.frames)
                    {
                        if(frame?.spriteCollection?.spriteDefinitions != null && frame.spriteId >= 0 && frame.spriteId < frame.spriteCollection.spriteDefinitions.Length)
                        {
                            var definition = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                            ETGMod.Assets.TextureMap.TryGetValue("sprites/" + frame.spriteCollection.name + "/" + definition.name, out var texture);
                            if(texture != null && definition != null)
                            {
                                var pixelOffset = texture.TrimTexture();
                                RuntimeAtlasSegment ras = ETGMod.Assets.Packer.Pack(texture); //pack your resources beforehand or the outlines will turn out weird

                                Material material = new Material(definition.material);
                                material.mainTexture = ras.texture;
                                definition.uvs = ras.uvs;
                                definition.material = material;
                                if(definition.materialInst != null)
                                {
                                    definition.materialInst = new Material(material);
                                }
                                float num = texture.width * 0.0625f;
                                float num2 = texture.height * 0.0625f;
                                definition.position0 = new Vector3(0f, 0f, 0f);
                                definition.position1 = new Vector3(num, 0f, 0f);
                                definition.position2 = new Vector3(0f, num2, 0f);
                                definition.position3 = new Vector3(num, num2, 0f);
                                definition.boundsDataCenter = definition.untrimmedBoundsDataCenter = new Vector3(num / 2f, num2 / 2f, 0f);
                                definition.boundsDataExtents = definition.untrimmedBoundsDataExtents = new Vector3(num, num2, 0f);
                                definition.AddOffset(pixelOffset.ToVector2() / 16f);
                                ids.Add(new KeyValuePair<tk2dSpriteCollectionData, int>(frame.spriteCollection, frame.spriteId));
                            }
                        }
                    }
                }
            }
        }

        public static void AddOffset(this tk2dSpriteDefinition def, Vector2 offset, bool changesCollider = false)
        {
            float xOffset = offset.x;
            float yOffset = offset.y;
            def.position0 += new Vector3(xOffset, yOffset, 0);
            def.position1 += new Vector3(xOffset, yOffset, 0);
            def.position2 += new Vector3(xOffset, yOffset, 0);
            def.position3 += new Vector3(xOffset, yOffset, 0);
            def.boundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.boundsDataExtents += new Vector3(xOffset, yOffset, 0);
            //def.untrimmedBoundsDataCenter += new Vector3(xOffset, yOffset, 0);
            //def.untrimmedBoundsDataExtents += new Vector3(xOffset, yOffset, 0);
            if (def.colliderVertices != null && def.colliderVertices.Length > 0 && changesCollider)
            {
                def.colliderVertices[0] += new Vector3(xOffset, yOffset, 0);
            }
        }

        // totally not stolen from ccm code :)
        public static IntVector2 TrimTexture(this Texture2D orig)
        {
            RectInt bounds = orig.GetTrimmedBounds();
            Color[][] pixels = new Color[bounds.width][];

            for (int x = bounds.x; x < bounds.x + bounds.width; x++)
            {
                for (int y = bounds.y; y < bounds.y + bounds.height; y++)
                {
                    if(pixels[x - bounds.x] == null)
                    {
                        pixels[x - bounds.x] = new Color[bounds.height];
                    }
                    pixels[x - bounds.x][y - bounds.y] = orig.GetPixel(x, y);
                }
            }

            orig.Resize(bounds.width, bounds.height);

            for (int x = 0; x < bounds.width; x++)
            {
                for (int y = 0; y < bounds.height; y++)
                {
                    orig.SetPixel(x, y, pixels[x][y]);
                }
            }
            orig.Apply(false, false);
            return new IntVector2(bounds.x, bounds.y);
        }

        public static RectInt GetTrimmedBounds(this Texture2D t)
        {

            int xMin = t.width;
            int yMin = t.height;
            int xMax = 0;
            int yMax = 0;

            for (int x = 0; x < t.width; x++)
            {
                for (int y = 0; y < t.height; y++)
                {
                    if (t.GetPixel(x, y).a > 0)
                    {
                        if (x < xMin) xMin = x;
                        if (y < yMin) yMin = y;
                        if (x > xMax) xMax = x;
                        if (y > yMax) yMax = y;
                    }
                }
            }

            return new RectInt(xMin, yMin, xMax - xMin + 1, yMax - yMin + 1);
        }


        public static Texture2D GetReadableVersion(this Texture2D tex)
        {
            return tex.IsReadable() ? tex : tex.GetRW();
        }

        /// <summary>
        /// Adds a sprite from a definition to a collection
        /// </summary>
        /// <returns>The spriteID of the defintion in the collection</returns>
        public static int AddSpriteToCollection(tk2dSpriteDefinition spriteDefinition, tk2dSpriteCollectionData collection)
        {
            //Add definition to collection
            var defs = collection.spriteDefinitions;
            var newDefs = defs.Concat(new tk2dSpriteDefinition[] { spriteDefinition }).ToArray();
            collection.spriteDefinitions = newDefs;

            //Reset lookup dictionary
            FieldInfo f = typeof(tk2dSpriteCollectionData).GetField("spriteNameLookupDict", BindingFlags.Instance | BindingFlags.NonPublic);
            f.SetValue(collection, null);  //Set dictionary to null
            collection.InitDictionary(); //InitDictionary only runs if the dictionary is null
            return newDefs.Length - 1;
        }

        public static void ConstructOffsetsFromAnchor(this tk2dSpriteDefinition def, tk2dBaseSprite.Anchor anchor, Vector2? scale = null, bool fixesScale = false, bool changesCollider = true)
        {
            if (!scale.HasValue)
            {
                scale = new Vector2?(def.position3);
            }
            if (fixesScale)
            {
                Vector2 fixedScale = scale.Value - def.position0.XY();
                scale = new Vector2?(fixedScale);
            }
            float xOffset = 0;
            if (anchor == tk2dBaseSprite.Anchor.LowerCenter || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.UpperCenter)
            {
                xOffset = -(scale.Value.x / 2f);
            }
            else if (anchor == tk2dBaseSprite.Anchor.LowerRight || anchor == tk2dBaseSprite.Anchor.MiddleRight || anchor == tk2dBaseSprite.Anchor.UpperRight)
            {
                xOffset = -scale.Value.x;
            }
            float yOffset = 0;
            if (anchor == tk2dBaseSprite.Anchor.MiddleLeft || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.MiddleLeft)
            {
                yOffset = -(scale.Value.y / 2f);
            }
            else if (anchor == tk2dBaseSprite.Anchor.UpperLeft || anchor == tk2dBaseSprite.Anchor.UpperCenter || anchor == tk2dBaseSprite.Anchor.UpperRight)
            {
                yOffset = -scale.Value.y;
            }
            def.AddOffset(new Vector2(xOffset, yOffset), false);
            if (changesCollider && def.colliderVertices != null && def.colliderVertices.Length > 0)
            {
                float colliderXOffset = 0;
                if (anchor == tk2dBaseSprite.Anchor.LowerLeft || anchor == tk2dBaseSprite.Anchor.MiddleLeft || anchor == tk2dBaseSprite.Anchor.UpperLeft)
                {
                    colliderXOffset = (scale.Value.x / 2f);
                }
                else if (anchor == tk2dBaseSprite.Anchor.LowerRight || anchor == tk2dBaseSprite.Anchor.MiddleRight || anchor == tk2dBaseSprite.Anchor.UpperRight)
                {
                    colliderXOffset = -(scale.Value.x / 2f);
                }
                float colliderYOffset = 0;
                if (anchor == tk2dBaseSprite.Anchor.LowerLeft || anchor == tk2dBaseSprite.Anchor.LowerCenter || anchor == tk2dBaseSprite.Anchor.LowerRight)
                {
                    colliderYOffset = (scale.Value.y / 2f);
                }
                else if (anchor == tk2dBaseSprite.Anchor.UpperLeft || anchor == tk2dBaseSprite.Anchor.UpperCenter || anchor == tk2dBaseSprite.Anchor.UpperRight)
                {
                    colliderYOffset = -(scale.Value.y / 2f);
                }
                def.colliderVertices[0] += new Vector3(colliderXOffset, colliderYOffset, 0);
            }
        }

        /// <summary>
        /// Adds a sprite definition to the Ammonomicon sprite collection
        /// </summary>
        /// <returns>The spriteID of the defintion in the ammonomicon collection</returns>
        public static int AddToAmmonomicon(tk2dSpriteDefinition spriteDefinition)
        {
            return AddSpriteToCollection(spriteDefinition, ammonomiconCollection);
        }

        public static tk2dSpriteDefinition CopyDefinitionFrom(this tk2dSpriteDefinition other)
        {
            tk2dSpriteDefinition result = new tk2dSpriteDefinition
            {
                boundsDataCenter = new Vector3
                {
                    x = other.boundsDataCenter.x,
                    y = other.boundsDataCenter.y,
                    z = other.boundsDataCenter.z
                },
                boundsDataExtents = new Vector3
                {
                    x = other.boundsDataExtents.x,
                    y = other.boundsDataExtents.y,
                    z = other.boundsDataExtents.z
                },
                colliderConvex = other.colliderConvex,
                colliderSmoothSphereCollisions = other.colliderSmoothSphereCollisions,
                colliderType = other.colliderType,
                colliderVertices = other.colliderVertices,
                collisionLayer = other.collisionLayer,
                complexGeometry = other.complexGeometry,
                extractRegion = other.extractRegion,
                flipped = other.flipped,
                indices = other.indices,
                material = new Material(other.material),
                materialId = other.materialId,
                materialInst = new Material(other.materialInst),
                metadata = other.metadata,
                name = other.name,
                normals = other.normals,
                physicsEngine = other.physicsEngine,
                position0 = new Vector3
                {
                    x = other.position0.x,
                    y = other.position0.y,
                    z = other.position0.z
                },
                position1 = new Vector3
                {
                    x = other.position1.x,
                    y = other.position1.y,
                    z = other.position1.z
                },
                position2 = new Vector3
                {
                    x = other.position2.x,
                    y = other.position2.y,
                    z = other.position2.z
                },
                position3 = new Vector3
                {
                    x = other.position3.x,
                    y = other.position3.y,
                    z = other.position3.z
                },
                regionH = other.regionH,
                regionW = other.regionW,
                regionX = other.regionX,
                regionY = other.regionY,
                tangents = other.tangents,
                texelSize = new Vector2
                {
                    x = other.texelSize.x,
                    y = other.texelSize.y
                },
                untrimmedBoundsDataCenter = new Vector3
                {
                    x = other.untrimmedBoundsDataCenter.x,
                    y = other.untrimmedBoundsDataCenter.y,
                    z = other.untrimmedBoundsDataCenter.z
                },
                untrimmedBoundsDataExtents = new Vector3
                {
                    x = other.untrimmedBoundsDataExtents.x,
                    y = other.untrimmedBoundsDataExtents.y,
                    z = other.untrimmedBoundsDataExtents.z
                }
            };
            if (other.uvs != null)
            {
                List<Vector2> uvs = new List<Vector2>();
                foreach (Vector2 vector in other.uvs)
                {
                    uvs.Add(new Vector2
                    {
                        x = vector.x,
                        y = vector.y
                    });
                }
                result.uvs = uvs.ToArray();
            }
            else
            {
                result.uvs = null;
            }
            if (other.colliderVertices != null)
            {
                List<Vector3> colliderVertices = new List<Vector3>();
                foreach (Vector3 vector in other.colliderVertices)
                {
                    colliderVertices.Add(new Vector3
                    {
                        x = vector.x,
                        y = vector.y,
                        z = vector.z
                    });
                }
                result.colliderVertices = colliderVertices.ToArray();
            }
            else
            {
                result.colliderVertices = null;
            }
            return result;
        }

        public static tk2dSpriteCollectionData SetupCollection(this GameObject self)
        {
            tk2dSpriteCollectionData collection = self.AddComponent<tk2dSpriteCollectionData>();
            collection.spriteDefinitions = new tk2dSpriteDefinition[0];
            var name = (self.name + (self.name.ToLowerInvariant().Contains("collection") ? "" : "Collection")).Replace(" ", "");
            collection.spriteCollectionName = name;
            collection.assetName = name;
            return collection;
        }

        public static int AddToAmmonomicon(string spritePath)
        {
            return AddSpriteToCollection(spritePath, ammonomiconCollection, "tk2d/CutoutVertexColorTilted");
        }

        /// <summary>
        /// Constructs a new tk2dSpriteDefinition with the given texture
        /// </summary>
        /// <returns>A new sprite definition with the given texture</returns>
        public static tk2dSpriteDefinition ConstructDefinition(Texture2D texture, string shaderName, Vector3? overrideUntrimmedCenter = null, Vector3? overrideUntrimmedExtents = null)
        {
            RuntimeAtlasSegment ras = ETGMod.Assets.Packer.Pack(texture); //pack your resources beforehand or the outlines will turn out weird

            Material material = new(ShaderCache.Acquire(shaderName));
            material.mainTexture = ras.texture;
            //material.mainTexture = texture;

            var width = texture.width;
            var height = texture.height;

            var x = 0f;
            var y = 0f;

            var w = width / 16f;
            var h = height / 16f;

            var def = new tk2dSpriteDefinition
            {
                normals = new Vector3[] {
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
            },
                tangents = new Vector4[] {
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            },
                texelSize = new Vector2(1 / 16f, 1 / 16f),
                extractRegion = false,
                regionX = 0,
                regionY = 0,
                regionW = 0,
                regionH = 0,
                flipped = tk2dSpriteDefinition.FlipMode.None,
                complexGeometry = false,
                physicsEngine = tk2dSpriteDefinition.PhysicsEngine.Physics3D,
                colliderType = tk2dSpriteDefinition.ColliderType.None,
                collisionLayer = CollisionLayer.HighObstacle,
                position0 = new Vector3(x, y, 0f),
                position1 = new Vector3(x + w, y, 0f),
                position2 = new Vector3(x, y + h, 0f),
                position3 = new Vector3(x + w, y + h, 0f),
                material = material,
                materialInst = material,
                materialId = 0,
                //uvs = ETGMod.Assets.GenerateUVs(texture, 0, 0, width, height), //uv machine broke
                uvs = ras.uvs,
                boundsDataCenter = new Vector3(w / 2f, h / 2f, 0f),
                boundsDataExtents = new Vector3(w, h, 0f),
                untrimmedBoundsDataCenter = overrideUntrimmedCenter ?? new Vector3(w / 2f, h / 2f, 0f),
                untrimmedBoundsDataExtents = overrideUntrimmedExtents ?? new Vector3(w, h, 0f),
            };

            def.name = texture.name;
            return def;
        }

        public static tk2dSpriteCollectionData ConstructCollection(string name)
        {
            var collection = new tk2dSpriteCollectionData();
            UnityEngine.Object.DontDestroyOnLoad(collection);

            collection.assetName = name;
            collection.allowMultipleAtlases = false;
            collection.buildKey = 0x0ade;
            collection.dataGuid = "what even is this for";
            collection.spriteCollectionGUID = name;
            collection.spriteCollectionName = name;
            collection.spriteDefinitions = new tk2dSpriteDefinition[0];
            /*
            var material_arr = new Material[Textures.Length];

            for (int i = 0; i < Textures.Length; i++)
            {
                material_arr[i] = new Material(DefaultSpriteShader);
                material_arr[i].mainTexture = Textures[i];
            }

            collection.textures = Textures;
            collection.textureInsts = Textures;

            collection.materials = material_arr;
            collection.materialInsts = material_arr;

            collection.needMaterialInstance = false;
            collection.spriteDefinitions = ConstructDefinitions(material_arr);
            */

            return collection;
        }


        public static T GetCopyOf<T, T2>(this T2 comp, T other, bool includeFields = true, bool includeProperties = true) where T : Component where T2 : Component
        {
            if(comp != null && other != null)
            {
                Type type = comp.GetType();
                if (type != other.GetType()) 
                {
                    ETGModConsole.Log(" type mis-match");
                    return null; 
                } // type mis-match
                if (includeProperties)
                {
                    PropertyInfo[] pinfos = type.GetProperties();
                    foreach (var pinfo in pinfos)
                    {
                        if (pinfo.CanWrite)
                        {
                            try
                            {
                                pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                            }
                            catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                        }
                        else
                        {
                        }
                    }
                }
                if (includeFields)
                {
                    FieldInfo[] finfos = type.GetFields();
                    foreach (var finfo in finfos)
                    {
                        try
                        {
                            finfo.SetValue(comp, finfo.GetValue(other));
                        }
                        catch { }
                    }
                }
                return comp as T;
            }
            return null;
        }


        public static void SetFields(this Component comp, Component other, bool includeFields = true, bool includeProperties = true)
        {
            if (comp != null && other != null)
            {
                Type type = comp.GetType();
                if (type != other.GetType())
                {
                    ETGModConsole.Log(" type mis-match");
                    return;
                } // type mis-match
                if (includeProperties)
                {
                    PropertyInfo[] pinfos = type.GetProperties();
                    foreach (var pinfo in pinfos)
                    {
                        if (pinfo.CanWrite)
                        {
                            try
                            {
                                pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                            }
                            catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                        }
                        else
                        {
                        }
                    }
                }
                if (includeFields)
                {
                    FieldInfo[] finfos = type.GetFields();
                    foreach (var finfo in finfos)
                    {
                        try
                        {
                            finfo.SetValue(comp, finfo.GetValue(other));
                        }
                        catch { }
                    }
                }
            }
        }

        public static T AddComponent<T>(this GameObject go, T toAdd, bool includeFields = true, bool includeProperties = true) where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd, includeFields, includeProperties) as T;
        }

        /*
        public static GameObject SpriteObjectFromTexture(Texture2D texture)
        {
            Rect region = new Rect(0, 0, texture.width, texture.height);

            var obj = tk2dSprite.CreateFromTexture(texture, tk2dSpriteCollectionSize.PixelsPerMeter(16), region, Vector2.zero);

            var collection = obj.GetComponent<tk2dSprite>().Collection;
            UnityEngine.Object.DontDestroyOnLoad(collection);

            var def = collection.spriteDefinitions[0];
            def.ReplaceTexture(texture);
            def.name = texture.name;

            Serializer.Serialize(def, texture.name + "_latedef");
            return obj;
        }
        */

        /*
       /// <summary>
       /// Adds a new sprite definition to the ammonomicon's collection
       /// </summary>
       /// <returns>The sprite ID of the newly added definition</returns>
       public static int AddSpriteToAmmonomicon(tk2dSpriteDefinition definition)
       {
           //Add sprite to definitions
           var iconCollection = AmmonomiconController.ForceInstance.EncounterIconCollection;
           var defs = iconCollection.spriteDefinitions;
           var newDefs = defs.Concat(new tk2dSpriteDefinition[] { definition }).ToArray();
           iconCollection.spriteDefinitions = newDefs;

           //Reset lookup dictionary
           FieldInfo f = typeof(tk2dSpriteCollectionData).GetField("spriteNameLookupDict", BindingFlags.Instance | BindingFlags.NonPublic);
           f.SetValue(iconCollection, null);  //Set dictionary to null
           iconCollection.InitDictionary(); //InitDictionary only runs if the dictionary is null

           return newDefs.Length - 1;
       }
       */
    }
}
