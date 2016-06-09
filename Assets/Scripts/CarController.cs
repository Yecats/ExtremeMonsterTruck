using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    [Header("Game Objects")]
    public List<AxleInfo> AxleInformation;
    public List<ParticleSystem> EnvironmentParticles;

    [Header("Properties")]
    public float MaxMotorTorque;
    public float MaxSteeringAngle;
    public Rigidbody CarRigidBody;

    private float _motorTorque;
    private float _breakTorque;
    private string _currentTexture;

    void Start()
    {

        CarRigidBody.centerOfMass = transform.FindChild("CenterOfMass").transform.localPosition;
        GetTerrainTextureName(AxleInformation[0].Wheels[0].transform.position);


    }

    public void Update()
    {
        if (GameManager.Instance.IsGameStarted())
        {
            GetTerrainTextureName(AxleInformation[0].Wheels[0].transform.position);
        }
    }

    public void FixedUpdate()
    {
        if (GameManager.Instance.IsGameStarted())
        {
            if (Input.GetAxis("Brake") != 0)
            {
                _motorTorque = 0;
                _breakTorque = 500;
            }

            if (Input.GetAxis("Vertical") != 0)
            {
                _breakTorque = 0;
                _motorTorque = MaxMotorTorque * Input.GetAxis("Vertical");
            }

            if (Input.GetAxis("Horizontal") != 0)
            {
                float steering = MaxSteeringAngle * Input.GetAxis("Horizontal");

                for (int i = 0; i < AxleInformation.Count; i++)
                {
                    if (AxleInformation[i].steering)
                    {
                        foreach (WheelCollider wheel in AxleInformation[i].Wheels)
                        {
                            wheel.steerAngle = steering;
                        }
                    }
                }
            }
        }
        else
        {
            _motorTorque = 0;
            _breakTorque = 500;
        }

        ApplyTorque();
    }

    /// <summary>
    /// Applies motor and/or break torque to the vehicle
    /// </summary>
    private void ApplyTorque()
    {
        foreach (AxleInfo axleInfo in AxleInformation)
        {
            //Adjust torque for wheels that are tied to the motor
            if (axleInfo.motor)
            {
                foreach (WheelCollider wheel in axleInfo.Wheels)
                {
                    Vector3 position;
                    Quaternion rotation;

                    wheel.GetWorldPose(out position, out rotation);

                    wheel.motorTorque = _motorTorque;
                    wheel.brakeTorque = _breakTorque;

                    ApplyLocalPositionToVisuals(wheel);
                }
            }
            //Adjust visual for wheels not tied to the motor
            else 
            {
                foreach (WheelCollider wheel in axleInfo.Wheels)
                {
                    ApplyLocalPositionToVisuals(wheel);
                }
            }

            //Turn on the particles and set their rate based on the speed of the car
            foreach (ParticleSystem particle in EnvironmentParticles)
            {

                double currentSpeed = CarRigidBody.velocity.magnitude * 2.237;
                ParticleSystem.EmissionModule emission;

                if (particle.gameObject.tag.Contains(_currentTexture))
                {
                    emission = particle.emission;

                    if (currentSpeed > 1)
                    {
                        emission.rate = new ParticleSystem.MinMaxCurve((float)currentSpeed * 2);
                        emission.enabled = true;
                    }
                    else
                    {
                        emission.enabled = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Obtains the name of the texture painted on a terrain based on the location provided
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <returns>Texture's Name</returns>
    private void GetTerrainTextureName(Vector3 position)
    {
        Vector3 terrainSize; 
        Vector2 alphaSize;
        TerrainData terrainData = Terrain.activeTerrain.terrainData;

        //Set terrain info
        terrainSize = Terrain.activeTerrain.terrainData.size;
        alphaSize.x = Terrain.activeTerrain.terrainData.alphamapWidth;
        alphaSize.y = Terrain.activeTerrain.terrainData.alphamapHeight;

        //Lookup Texture Info
        int x = (int)((position.x / terrainSize.x) * alphaSize.x + 0.5f);
        int y = (int)((position.z / terrainSize.z) * alphaSize.y + 0.5f);
        float[,,] terrainControl = Terrain.activeTerrain.terrainData.GetAlphamaps(x, y, 1, 1);

        for (int i = 0; i < terrainData.splatPrototypes.Length; i++)
        {
            if (terrainControl[0, 0, i] > .5f)
            {
                if (_currentTexture != terrainData.splatPrototypes[i].texture.name)
                {
                    _currentTexture = terrainData.splatPrototypes[i].texture.name;
                    SwapParticles();
                }

                break;
            }
        }
    }

    /// <summary>
    /// Apply the visual position / rotation to the wheel mesh
    /// </summary>
    /// <param name="collider"></param>
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        Transform visualWheel = collider.transform.parent.parent.transform.FindChild("Wheel_Models/" + collider.name);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    /// <summary>
    /// Swaps the particles that are rendering based on the current texture 
    /// </summary>
    private void SwapParticles()
    {
        ParticleSystem.EmissionModule emission;

        foreach (ParticleSystem particle in EnvironmentParticles)
        {
            emission = particle.emission;

            if (particle.gameObject.tag.Contains(_currentTexture))
            {
                emission.enabled = true;
            }
            else
            {
                emission.enabled = false;
            }
        }
    }

}