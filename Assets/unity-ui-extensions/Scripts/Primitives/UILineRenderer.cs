/// Credit jack.sydorenko 
/// Sourced from - http://forum.unity3d.com/threads/new-ui-and-line-drawing.253772/

using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI.Extensions
{
 	[AddComponentMenu("UI/Extensions/Primitives/UILineRenderer")]
   public class UILineRenderer : MaskableGraphic
    {
        [SerializeField]
        Texture m_Texture;
        [SerializeField]
        Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

        public float LineThickness = 2;
		[SerializeField]
		private Vector2[] Points;

        public override Texture mainTexture
        {
            get
            {
                return m_Texture == null ? s_WhiteTexture : m_Texture;
            }
        }

        /// <summary>
        /// Texture to be used.
        /// </summary>
        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value)
                    return;

                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// UV rectangle used by the texture.
        /// </summary>
        public Rect uvRect
        {
            get
            {
                return m_UVRect;
            }
            set
            {
                if (m_UVRect == value)
                    return;
                m_UVRect = value;
                SetVerticesDirty();
            }
        }

		public void AddPoint(Vector2 point)
		{
			Points = Points.Concat(Enumerable.Repeat(point, 1)).ToArray();
			SetVerticesDirty();
		}

		public void Clear()
		{
			Points = new Vector2[0];
			SetVerticesDirty();
		}

		public void SetPoints(Vector2[] points)
		{
			Points = points.ToArray();
			SetVerticesDirty();
		}

        protected override void OnPopulateMesh(VertexHelper vh)
        {
			vh.Clear();

			if (Points.Length < 2)
				return;

            var sizeX = rectTransform.rect.width;
            var sizeY = rectTransform.rect.height;
            var offsetX = -rectTransform.pivot.x * rectTransform.rect.width;
            var offsetY = -rectTransform.pivot.y * rectTransform.rect.height;

			var sizeVector = new Vector2(sizeX, sizeY);
			var offsetVector = new Vector2(offsetX, offsetY);

			for (var i = 0; i < Points.Length - 1; i++)
			{
				var current = new Vector2(Points[i].x * sizeVector.x, Points[i].y * sizeVector.y) + offsetVector;
				var next = new Vector2(Points[i + 1].x * sizeVector.x, Points[i + 1].y * sizeVector.y) + offsetVector;

				var line = next - current;
				var normalized = line.normalized;
				var up = normalized.Rotate(-90) * LineThickness / 2;

				var vertices = new Vector2[4];
				vertices[0] = current + up;
				vertices[1] = next + up;
				vertices[2] = next - up;
				vertices[3] = current - up;

				var uvs = new Vector2[4] { Vector2.zero, new Vector2(1, 0), Vector2.one, new Vector2(0, 1) };
				vh.AddUIVertexQuad(CreateUIVertexQuad(vertices, uvs));
			}
        }

        protected UIVertex[] CreateUIVertexQuad(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
                for (int i = 0; i < vertices.Length; i++)
                {
                    var vert = UIVertex.simpleVert;
                    vert.color = color;
                    vert.position = vertices[i];
                    vert.uv0 = uvs[i];
                    vbo[i] = vert;
                }
            return vbo;
        }

        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }
    }
}