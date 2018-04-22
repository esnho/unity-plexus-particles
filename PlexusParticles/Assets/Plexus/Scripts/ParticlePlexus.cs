using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inspired by this tutorial https://www.youtube.com/watch?v=ruNPkuYT1Ck

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePlexus : MonoBehaviour {

	public float MaxDistance;

	new ParticleSystem particleSystem;
	ParticleSystem.Particle[] particles;

	ParticleSystem.MainModule particleSystemMainModule;

	public LineRenderer lineRendererTemplate;
	List<LineRenderer> lineRenderers = new List<LineRenderer>();


	public int lrVertexCount = 2;
	public float lrVertexNoise = 0.3f;

	public int maxLines = 300;

	public bool remainingLifeKills = true;

	public bool remainingLifeInfluenceWidth = true;

	void Start () {
		particleSystem = GetComponent<ParticleSystem> ();
		particleSystemMainModule = particleSystem.main;

		if (lrVertexCount < 2) {
			lrVertexCount = 2;
		}
	}
	
	void LateUpdate ()
    {
        int maxParticle = particleSystemMainModule.maxParticles;

        if (particles == null || particles.Length < maxParticle)
        {
            particles = new ParticleSystem.Particle[maxParticle];
        }

        particleSystem.GetParticles(particles);
        int particleCount = particleSystem.particleCount;

        int currenLineRendererIndex = 0;
        int lineRendererCount = lineRenderers.Count;

        float maxDistanceSqr = MaxDistance * MaxDistance;
        
        for (int i = 0; i < particleCount; i++)
        {

            Vector3 p1_pos = particles[i].position;
            float p1_normRemainingLife = particles[i].remainingLifetime / particles[i].startLifetime;

            for (int j = i + 1; j < particleCount; j++)
            {

                if (currenLineRendererIndex > maxLines)
                    return;

                Vector3 p2_pos = particles[j].position;

                float distanceSqr = Vector3.SqrMagnitude(p1_pos - p2_pos);

                if (distanceSqr <= maxDistanceSqr)
                {

                    float p2_normRemainingLife = particles[j].remainingLifetime / particles[j].startLifetime;

                    float lifeToDeath = Mathf.Min(p2_normRemainingLife, p1_normRemainingLife);

                    if (remainingLifeKills)
                    {

                        lifeToDeath *= 2f;
                        lifeToDeath -= 1f;

                        if (lifeToDeath <= 0)
                            break;

                    }

                    InstantiateNewLineRendererIfNecessary(currenLineRendererIndex, lineRendererCount);

                    DrawLineRenderer(currenLineRendererIndex, p1_pos, p2_pos, distanceSqr, maxDistanceSqr);
                    SetLineWidth(currenLineRendererIndex, maxDistanceSqr, distanceSqr, lifeToDeath);

                    currenLineRendererIndex++;
                }
            }
        }

        DisabelRemainingLineRenderers(currenLineRendererIndex, lineRendererCount);
    }

    private void InstantiateNewLineRendererIfNecessary(int currenLineRendererIndex, int lineRendererCount)
    {
        bool isNewLineRenderer = currenLineRendererIndex >= lineRendererCount;

        if (isNewLineRenderer)
        {
            LineRenderer lineRenderer = Instantiate(lineRendererTemplate, transform, false);
            lineRenderers.Add(lineRenderer);
        }
    }

    private void DrawLineRenderer(int index, Vector3 p1_pos, Vector3 p2_pos, float distanceSqr, float maxDistanceSqr)
    {
        LineRenderer lineRenderer = lineRenderers[index];

        lineRenderer.numPositions = lrVertexCount;

        float noiseAmplitude = ((float)(distanceSqr) / (float)maxDistanceSqr);
        Vector3[] points = CalculatePoints(p1_pos, p2_pos, noiseAmplitude);

        lineRenderer.SetPositions(points);

        lineRenderer.enabled = true;
    }

    private Vector3[] CalculatePoints(Vector3 p1_pos, Vector3 p2_pos, float noiseAmplitude)
    {
        Vector3[] points = new Vector3[lrVertexCount];
        points[0] = p1_pos;
        points[lrVertexCount - 1] = p2_pos;
        for (int k = 1; k < lrVertexCount - 1; k++)
        {
            Vector3 tmp = Vector3.Lerp(p1_pos, p2_pos, (float)k / (float)lrVertexCount);
            float kx = Random.Range(lrVertexNoise, -lrVertexNoise) * noiseAmplitude;
            float ky = Random.Range(lrVertexNoise, -lrVertexNoise) * noiseAmplitude;
            float kz = Random.Range(lrVertexNoise, -lrVertexNoise) * noiseAmplitude;
            tmp += new Vector3(kx, ky, kz);
            points[k] = tmp;
        }

        return points;
    }

    private void SetLineWidth(int currenLineRendererIndex, float maxDistanceSqr, float distanceSqr, float lifeInfluence)
    {
        if (!remainingLifeInfluenceWidth)
        {
            lifeInfluence = 1f;
        }
        float distanceInfluence = ((float)(maxDistanceSqr - distanceSqr) / (float)maxDistanceSqr);
        lineRenderers[currenLineRendererIndex].widthMultiplier = lineRendererTemplate.widthMultiplier * distanceInfluence * lifeInfluence;
    }

    private void DisabelRemainingLineRenderers(int currenLineRendererIndex, int lineRendererCount)
    {
        for (int i = currenLineRendererIndex; i < lineRendererCount; i++)
        {
            lineRenderers[i].enabled = false;
        }
    }

    float[,] calcNoise(float[,] noise) {
		float[,] tmp = noise;
		float y = 0;
		while (y < noise.GetUpperBound(0)) {
			float x = 0;
			while (x < noise.GetUpperBound(1)) {
				float sample = Mathf.PerlinNoise (y, x);
				tmp [(int)y, (int)x] = sample;
				x++;
			}
			y++;
		}

		return tmp;
	}
}
