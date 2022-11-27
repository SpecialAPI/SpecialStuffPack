using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

namespace SpecialStuffPack.CursorAPI
{
	/// <summary>
	/// The class that has all tool methods used by CursorAPI.
	/// </summary>
    public static class CursorTools
    {
		/// <summary>
		/// Builds and adds a new <see cref="dfAtlas.ItemInfo"/> to <paramref name="atlas"/> with the texture of <paramref name="tex"/> and the name of <paramref name="name"/>.
		/// </summary>
		/// <param name="atlas">The <see cref="dfAtlas"/> to add the new <see cref="dfAtlas.ItemInfo"/> to.</param>
		/// <param name="tex">The texture of the new <see cref="dfAtlas.ItemInfo"/>.</param>
		/// <param name="name">The name of the new <see cref="dfAtlas.ItemInfo"/>. If <see langword="null"/>, it will default to <paramref name="tex"/>'s name.</param>
		/// <returns>The built <see cref="dfAtlas.ItemInfo"/>.</returns>
		public static dfAtlas.ItemInfo AddNewItemToAtlas(this dfAtlas atlas, Texture2D tex, string name = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = tex.name;
			}
            if (atlas[name] != null)
            {
				return atlas[name];
            }
			dfAtlas.ItemInfo item = new dfAtlas.ItemInfo
			{
				border = new RectOffset(),
				deleted = false,
				name = name,
				region = atlas.FindFirstValidEmptySpace(new IntVector2(tex.width, tex.height)),
				rotated = false,
				sizeInPixels = new Vector2(tex.width, tex.height),
				texture = tex,
				textureGUID = name
			};
			int startPointX = Mathf.RoundToInt(item.region.x * atlas.Texture.width);
			int startPointY = Mathf.RoundToInt(item.region.y * atlas.Texture.height);
			for (int x = startPointX; x < Mathf.RoundToInt(item.region.xMax * atlas.Texture.width); x++)
			{
				for (int y = startPointY; y < Mathf.RoundToInt(item.region.yMax * atlas.Texture.height); y++)
				{
					atlas.Texture.SetPixel(x, y, tex.GetPixel(x - startPointX, y - startPointY));
				}
			}
			atlas.Texture.Apply();
			atlas.AddItem(item);
			return item;
		}

		/// <summary>
		/// Converts a list of the type <typeparamref name="T"/> to a list of the type <typeparamref name="T2"/> using <paramref name="convertor"/>.
		/// </summary>
		/// <typeparam name="T">The type of the <paramref name="self"/> list.</typeparam>
		/// <typeparam name="T2">The type to convert the <paramref name="self"/> list to.</typeparam>
		/// <param name="self">The original list.</param>
		/// <param name="convertor">A delegate that converts an element of type <typeparamref name="T"/> to an element of a type <typeparamref name="T2"/>.</param>
		/// <returns>The converted list of type <typeparamref name="T2"/></returns>
		public static List<T2> Convert<T, T2>(this List<T> self, Func<T, T2> convertor)
		{
			List<T2> result = new List<T2>();
			foreach (T element in self)
			{
				result.Add(convertor(element));
			}
			return result;
		}

		/// <summary>
		/// Gets the pixel regions of <paramref name="atlas"/>.
		/// </summary>
		/// <param name="atlas">The <see cref="dfAtlas"/> to get the pixel regions from.</param>
		/// <returns>A list with all pixel regions in <paramref name="atlas"/></returns>
		public static List<RectInt> GetPixelRegions(this dfAtlas atlas)
		{
			return atlas.Items.Convert(delegate (dfAtlas.ItemInfo item)
			{
				return new RectInt(Mathf.RoundToInt(item.region.x * atlas.Texture.width), Mathf.RoundToInt(item.region.y * atlas.Texture.height), Mathf.RoundToInt(item.region.width * atlas.Texture.width),
					Mathf.RoundToInt(item.region.height * atlas.Texture.height));
			});
		}

		/// <summary>
		/// Adds <paramref name="element"/> to <paramref name="array"/>.
		/// </summary>
		/// <typeparam name="T">The type of the element to add.</typeparam>
		/// <param name="array">The array to which <paramref name="element"/> will be added.</param>
		/// <param name="element">The element to add to <paramref name="array"/>.</param>
		public static void Add<T>(ref T[] array, T element)
        {
			List<T> list = array.ToList();
			list.Add(element);
			array = list.ToArray();
        }

		/// <summary>
		/// Removes <paramref name="element"/> from <paramref name="array"/>.
		/// </summary>
		/// <typeparam name="T">The type of the element to remove.</typeparam>
		/// <param name="array">The array from which <paramref name="element"/> will be removed.</param>
		/// <param name="element">The element to remove from <paramref name="array"/>.</param>
		public static void Remove<T>(ref T[] array, T element)
		{
			List<T> list = array.ToList();
			list.Remove(element);
			array = list.ToArray();
		}

		/// <summary>
		/// Converts <paramref name="vector"/> to a <see cref="Vector2Int"/>.
		/// </summary>
		/// <param name="vector">The <see cref="IntVector2"/> to convert.</param>
		/// <returns><paramref name="vector"/> converted to <see cref="Vector2Int"/>.</returns>
		public static Vector2Int ToVector2Int(this IntVector2 vector)
		{
			return new Vector2Int(vector.x, vector.y);
		}

		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="self"/> overlaps with <paramref name="other"/>.
		/// </summary>
		/// <param name="self">This <see cref="RectInt"/>.</param>
		/// <param name="other">The other <see cref="RectInt"/></param>
		/// <returns><see langword="true"/> if <paramref name="self"/> overlaps with <paramref name="other"/>.</returns>
		public static bool Overlaps(this RectInt self, RectInt other)
		{
			return other.xMax > self.xMin && other.xMin < self.xMax && other.yMax > self.yMin && other.yMin < self.yMax;
		}

		/// <summary>
		/// Gets the first empty space in <paramref name="atlas"/> that has at least the size of <paramref name="pixelScale"/>.
		/// </summary>
		/// <param name="atlas">The <see cref="dfAtlas"/> to find the empty space in.</param>
		/// <param name="pixelScale">The required size of the empty space.</param>
		/// <returns>The rect of the empty space divided by the atlas texture's size.</returns>
		public static Rect FindFirstValidEmptySpace(this dfAtlas atlas, IntVector2 pixelScale)
		{
			if (atlas == null || atlas.Texture == null || !atlas.Texture.IsReadable())
			{
				return new Rect(0f, 0f, 0f, 0f);
			}
			Vector2Int point = new Vector2Int(0, 0);
			int pointIndex = -1;
			List<RectInt> rects = atlas.GetPixelRegions();
			while (true)
			{
				bool shouldContinue = false;
				foreach (RectInt rint in rects)
				{
					if (rint.Overlaps(new RectInt(point, pixelScale.ToVector2Int())))
					{
						shouldContinue = true;
						pointIndex++;
						if (pointIndex >= rects.Count)
						{
							return new Rect(0f, 0f, 0f, 0f);
						}
						point = rects[pointIndex].max + Vector2Int.one;
						if (point.x > atlas.Texture.width || point.y > atlas.Texture.height)
						{
							atlas.ResizeAtlas(new IntVector2(atlas.Texture.width * 2, atlas.Texture.height * 2));
						}
						break;
					}
					bool shouldBreak = false;
					foreach (RectInt rint2 in rects)
					{
						RectInt currentRect = new RectInt(point, pixelScale.ToVector2Int());
						if (rint2.x < currentRect.x || rint2.y < currentRect.y)
						{
							continue;
						}
						else
						{
							if (currentRect.Overlaps(rint2))
							{
								shouldContinue = true;
								shouldBreak = true;
								pointIndex++;
								if (pointIndex >= rects.Count)
								{
									return new Rect(0f, 0f, 0f, 0f);
								}
								point = rects[pointIndex].max + Vector2Int.one;
								if (point.x > atlas.Texture.width || point.y > atlas.Texture.height)
								{
									atlas.ResizeAtlas(new IntVector2(atlas.Texture.width * 2, atlas.Texture.height * 2));
								}
								break;
							}
						}
					}
					if (shouldBreak)
					{
						break;
					}
				}
				if (shouldContinue)
				{
					continue;
				}
				RectInt currentRect2 = new RectInt(point, pixelScale.ToVector2Int());
				if (currentRect2.xMax > atlas.Texture.width || currentRect2.yMax > atlas.Texture.height)
				{
					atlas.ResizeAtlas(new IntVector2(atlas.Texture.width * 2, atlas.Texture.height * 2));
				}
				break;
			}
			RectInt currentRect3 = new RectInt(point, pixelScale.ToVector2Int());
			Rect rect = new Rect((float)currentRect3.x / atlas.Texture.width, (float)currentRect3.y / atlas.Texture.height, (float)currentRect3.width / atlas.Texture.width, (float)currentRect3.height / atlas.Texture.height);
			return rect;
		}

		/// <summary>
		/// Resizes <paramref name="atlas"/> and all of it's <see cref="dfAtlas.ItemInfo"/>s.
		/// </summary>
		/// <param name="atlas">The <see cref="dfAtlas"/> to resize/</param>
		/// <param name="newDimensions"><paramref name="atlas"/>'s new size.</param>
		public static void ResizeAtlas(this dfAtlas atlas, IntVector2 newDimensions)
		{
			Texture2D tex = atlas.Texture;
			if (!tex.IsReadable())
			{
				return;
			}
			if (tex.width == newDimensions.x && tex.height == newDimensions.y)
			{
				return;
			}
			foreach (dfAtlas.ItemInfo item in atlas.Items)
			{
				if (item.region != null)
				{
					item.region.x = (item.region.x * tex.width) / newDimensions.x;
					item.region.y = (item.region.y * tex.height) / newDimensions.y;
					item.region.width = (item.region.width * tex.width) / newDimensions.x;
					item.region.height = (item.region.height * tex.height) / newDimensions.y;
				}
			}
			tex.ResizeBetter(newDimensions.x, newDimensions.y);
			atlas.Material.SetTexture("_MainTex", tex);
		}

		/// <summary>
		/// Resizes <paramref name="tex"/> without it losing it's pixel information.
		/// </summary>
		/// <param name="tex">The <see cref="Texture2D"/> to resize.</param>
		/// <param name="width">The <paramref name="tex"/>'s new width.</param>
		/// <param name="height">The <paramref name="tex"/>'s new height.</param>
		/// <returns></returns>
		public static bool ResizeBetter(this Texture2D tex, int width, int height)
		{
			if (tex.IsReadable())
			{
				Color[][] pixels = new Color[Math.Min(tex.width, width)][];
				for (int x = 0; x < Math.Min(tex.width, width); x++)
				{
					for (int y = 0; y < Math.Min(tex.height, height); y++)
					{
						if (pixels[x] == null)
						{
							pixels[x] = new Color[Math.Min(tex.height, height)];
						}
						pixels[x][y] = tex.GetPixel(x, y);
					}
				}
				bool result = tex.Resize(width, height);
				for (int x = 0; x < tex.width; x++)
				{
					for (int y = 0; y < tex.height; y++)
					{
						bool isInOrigTex = false;
						if (x < pixels.Length)
						{
							if (y < pixels[x].Length)
							{
								isInOrigTex = true;
								tex.SetPixel(x, y, pixels[x][y]);
							}
						}
						if (!isInOrigTex)
						{
							tex.SetPixel(x, y, Color.clear);
						}
					}
				}
				tex.Apply();
				return result;
			}
			return tex.Resize(width, height);
		}

		/// <summary>
		/// The max size of <see cref="GameUIRoot"/>'s atlas texture.
		/// </summary>
		public static int MaxAtlasTextureSize
		{
			get
			{
				return 16384;
			}
		}

		private static string[] BundlePrereqs;
        private static bool m_initialized;
    }
}
