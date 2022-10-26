using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
	public class BetterSquishyBounceWiggler : BraveBehaviour
	{
		public bool WiggleHold
		{
			get
			{
				return m_wiggleHold;
			}
			set
			{
				if (value && !m_wiggleHold)
				{
					if (this)
					{
						gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
					}
					ResetWiggle();
				}
				else if (!value && m_wiggleHold && this)
				{
					gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
				}
				m_wiggleHold = value;
			}
		}

		public void Awake()
		{
			m_sprite = GetComponent<tk2dBaseSprite>();
		}

		public void Start()
		{
			if (!m_sprite)
			{
				enabled = false;
			}
			Bounds bounds = m_sprite.GetBounds();
			m_spriteDimensions = new IntVector2(Mathf.RoundToInt(bounds.size.x / 0.0625f), Mathf.RoundToInt(bounds.size.y / 0.0625f));
			transform.position = transform.position.Quantize(0.0625f);
			if (specRigidbody)
			{
				specRigidbody.Reinitialize();
			}
			gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
			bounce = StartCoroutine(DoSquishyBounceWiggle());
		}

		public void StopBounce()
        {
			if(bounce != null)
            {
				StopCoroutine(bounce);
				ResetWiggle();
				bounce = null;
            }
        }

		public override void OnDestroy()
		{
			base.OnDestroy();
		}

		public void ResetWiggle()
		{
			if (m_sprite == null)
			{
				return;
			}
			MeshFilter component = GetComponent<MeshFilter>();
			Mesh mesh = component.mesh;
			Vector3[] vertices = mesh.vertices;
			Vector2[] uv = mesh.uv;
			Vector2 zero = Vector2.zero;
			Vector2 one = Vector2.one;
			Vector3 one2 = Vector3.one;
			Vector3 zero2 = Vector3.zero;
			SetClippedGeometry(m_sprite.GetCurrentSpriteDef(), vertices, uv, zero2, 0, one2, zero, one);
			Vector3[] normals = mesh.normals;
			Color[] colors = mesh.colors;
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.normals = normals;
			mesh.colors = colors;
			int[] array = new int[6];
			tk2dSpriteGeomGen.SetClippedSpriteIndices(array, 0, 0, m_sprite.GetCurrentSpriteDef());
			mesh.triangles = array;
			mesh.RecalculateBounds();
			(m_sprite as tk2dSprite).mesh = mesh;
			component.mesh = mesh;
		}

		private IEnumerator DoSquishyBounceWiggle()
		{
			MeshFilter mf = GetComponent<MeshFilter>();
			Mesh sourceMesh = mf.mesh;
			Vector3[] vertices = sourceMesh.vertices;
			Vector2[] uvs = sourceMesh.uv;
			float horizontalPercentagePixel = 1f / (float)m_spriteDimensions.x;
			float verticalPercentagePixel = 1f / (float)m_spriteDimensions.y;
			int[] bottomOffsets = new int[5];
			int[] upTranslations = new int[]
			{
			0,
			-3,
			1,
			2,
			-1
			};
			float[] array = new float[]
			{
			1f,
			1f,
			0f,
			1f,
			1f
			};
			array[2] = 1f - horizontalPercentagePixel * 2f;
			float[] horizontalScales = array;
			float[] array2 = new float[]
			{
			1f,
			0f,
			1f,
			1f,
			1f
			};
			array2[1] = 1f - verticalPercentagePixel * 2f;
			float[] verticalScales = array2;
			float[] delays = new float[]
			{
			0.8f,
			0.1f,
			0.1f,
			0.1f,
			0.1f
			};
			for (; ; )
			{
				for (int i = 0; i < 5; i++)
				{
					if (WiggleHold)
					{
						i = 0;
					}
					bool hasOutlines = SpriteOutlineManager.HasOutline(m_sprite);
					tk2dBaseSprite[] outlineSprites = (!hasOutlines) ? null : SpriteOutlineManager.GetOutlineSprites<tk2dBaseSprite>(m_sprite);
					Vector2 clipBottomLeft = new(0f, (float)bottomOffsets[i] * verticalPercentagePixel);
					Vector2 clipTopRight = new(1f, 1f);
					Vector3 scale = new(horizontalScales[i], verticalScales[i], 1f);
					Vector3 translation = new(0.0625f * ((1f - horizontalScales[i]) / 2f / horizontalPercentagePixel), 0.0625f * (float)upTranslations[i], 0f);
					SetClippedGeometry(m_sprite.GetCurrentSpriteDef(), vertices, uvs, translation, 0, scale, clipBottomLeft, clipTopRight);
					Vector3[] normals = sourceMesh.normals;
					Color[] colors = sourceMesh.colors;
					sourceMesh.Clear();
					sourceMesh.vertices = vertices;
					sourceMesh.uv = uvs;
					sourceMesh.normals = normals;
					sourceMesh.colors = colors;
					int[] indices = new int[6];
					tk2dSpriteGeomGen.SetClippedSpriteIndices(indices, 0, 0, m_sprite.GetCurrentSpriteDef());
					sourceMesh.triangles = indices;
                    sourceMesh.RecalculateBounds();
					(m_sprite as tk2dSprite).mesh = sourceMesh;
                    mf.mesh = sourceMesh;
					if (hasOutlines)
					{
						if (outlineSprites.Length == 1)
						{
							outlineSprites[0].scale = scale;
							outlineSprites[0].transform.localPosition = Vector3.Scale(translation, scale).WithZ(outlineSprites[0].transform.localPosition.z);
							SpriteOutlineManager.HandleSpriteChanged(outlineSprites[0]);
						}
						else
						{
							for (int j = 0; j < outlineSprites.Length; j++)
							{
								outlineSprites[j].scale = scale;
								outlineSprites[j].transform.localPosition = Vector3.Scale(IntVector2.Cardinals[j].ToVector3() * 0.0625f + translation, scale).WithZ(outlineSprites[j].transform.localPosition.z);
								SpriteOutlineManager.HandleSpriteChanged(outlineSprites[j]);
							}
						}
						m_sprite.UpdateZDepth();
					}
					float targetDelay = delays[i];
					float delayElapsed = 0f;
					while (delayElapsed < targetDelay)
					{
						delayElapsed += BraveTime.DeltaTime;
						if (i != 0)
						{
							base.transform.position = base.transform.position.Quantize(0.0625f);
						}
						yield return null;
					}
					if (i == 0)
					{
						while (WiggleHold)
						{
							if (i != 0)
							{
								base.transform.position = base.transform.position.Quantize(0.0625f);
							}
							yield return null;
						}
					}
				}
			}
		}

		private void SetClippedGeometry(tk2dSpriteDefinition spriteDef, Vector3[] pos, Vector2[] uv, Vector3 translation, int offset, Vector3 scale, Vector2 clipBottomLeft, Vector2 clipTopRight)
		{
			Vector2 vector = clipBottomLeft;
			Vector2 vector2 = clipTopRight;
			Vector3 position = spriteDef.position0;
			Vector3 position2 = spriteDef.position3;
			Vector3 vector3 = new(Mathf.Lerp(position.x, position2.x, vector.x) * scale.x, Mathf.Lerp(position.y, position2.y, vector.y) * scale.y, position.z * scale.z);
			Vector3 vector4 = new(Mathf.Lerp(position.x, position2.x, vector2.x) * scale.x, Mathf.Lerp(position.y, position2.y, vector2.y) * scale.y, position.z * scale.z);
			pos[offset] = new Vector3(vector3.x, vector3.y, vector3.z) + translation;
			pos[offset + 1] = new Vector3(vector4.x, vector3.y, vector3.z) + translation;
			pos[offset + 2] = new Vector3(vector3.x, vector4.y, vector3.z) + translation;
			pos[offset + 3] = new Vector3(vector4.x, vector4.y, vector3.z) + translation;
			if (m_sprite.ShouldDoTilt)
			{
				for (int i = offset; i < offset + 4; i++)
				{
					if (m_sprite.IsPerpendicular)
					{
						int num = i;
						pos[num].z = pos[num].z - pos[i].y;
					}
					else
					{
						int num2 = i;
						pos[num2].z = pos[num2].z + pos[i].y;
					}
				}
			}
			if (spriteDef.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
			{
				Vector2 vector5 = new(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector.x));
				Vector2 vector6 = new(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2.x));
				uv[offset] = new Vector2(vector5.x, vector5.y);
				uv[offset + 1] = new Vector2(vector5.x, vector6.y);
				uv[offset + 2] = new Vector2(vector6.x, vector5.y);
				uv[offset + 3] = new Vector2(vector6.x, vector6.y);
			}
			else if (spriteDef.flipped == tk2dSpriteDefinition.FlipMode.TPackerCW)
			{
				Vector2 vector7 = new(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector.x));
				Vector2 vector8 = new(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2.x));
				uv[offset] = new Vector2(vector7.x, vector7.y);
				uv[offset + 2] = new Vector2(vector8.x, vector7.y);
				uv[offset + 1] = new Vector2(vector7.x, vector8.y);
				uv[offset + 3] = new Vector2(vector8.x, vector8.y);
			}
			else
			{
				Vector2 vector9 = new(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector.x), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector.y));
				Vector2 vector10 = new(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2.x), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2.y));
				uv[offset] = new Vector2(vector9.x, vector9.y);
				uv[offset + 1] = new Vector2(vector10.x, vector9.y);
				uv[offset + 2] = new Vector2(vector9.x, vector10.y);
				uv[offset + 3] = new Vector2(vector10.x, vector10.y);
			}
		}

		private bool m_wiggleHold;
		private Coroutine bounce;
		protected tk2dBaseSprite m_sprite;
		protected IntVector2 m_spriteDimensions;
	}
}
