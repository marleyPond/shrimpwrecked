using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Director : MonoBehaviour {

    bool debug_autostart = false;                   //false
    bool debug_go_to_space = false;                  //false
    bool debug_short_levels = false;                 //false
    bool debug_instaboss = false;                   //false
    bool debug_unparalyze_shrimp_during_test = false;   //false

    bool debug_roll_credits_immediately = false;     //false

    public static bool debug_shrimp_can_game_over = true;  //true

    bool allowBeingsGoingDown = false;  
    int dirRandomMax = 4;

    bool inSpace = false;
    int activeSpaceBeings = 0;
    int maxSpaceBeings = 4;
    int spaceBeingsDestroyed = 0;
    int spaceBeingsToDestroy = 40;

    NUI nui;
    AudioManager am;

    string[] zoneNames = new string[]
    {
        "Epipelagic Zone",
        "Mesopelagic Zone",
        "Bathypelagic Zone",
        "Abyssopelagic Zone",
        "Hadalpelagic Zone",
        "The Cosmos"
    };

    float[] zoneLightAmount = new float[]
    {
        0.9f,
        0.4f,
        0.12f,
        0f,
        0f,
        1.5f
    };

    string[] zoneBosses = new string[]
    {
        "Saxophone Shark",
        "Front Whale",
        "Poser Shrimp",
        "Jamming Jellyfish",
        "Angler Fish",
        "Clockwork Beast \n Mothership"
    };

    float[] zoneBossCameraSize = new float[]
    {
        2.2f,
        3f,
        3.25f,
        3.25f,
        3.5f,
        3.25f
    };

    int fishAlive = 0;
    int octopusAlive = 0;
    int jellyfishAlive = 0;
    int blowfishAlive = 0;
    int whaleAlive = 0;

    List<GenericFish> fishPool;
    List<Turtle> turtlePool;
    List<Octopus> octopusPool;
    List<Jellyfish> jellyfishPool;
    List<Blowfish> blowfishPool;
    List<Whale> whalePool;
    List<WallJellyfish> wallJellyfishPool;
    List<MiniAnglerFish> miniAnglerFishPool;
    List<Eel> eelPool;

    List<SaxophoneShark> saxophoneSharkPool;
    List<PoserShrimp> poserShrimpPool;
    List<FrontWhale> frontWhalePool;
    List<MimicOctopus> mimicOctopusPool;
    List<UnspeakableBeast> unspeakableBeastPool;
    List<Mothership> mothershipPool;

    List<StarshipBeamer> beamerPool;
    List<StarshipDiscoPepper> discoPepperPool;

    MeshRenderer oceanBackground, spaceBackground;
    Light mainLight;
    ParticleSystem bubbles;

    List<Enemy> activeBosses;

    Shrimp shrimp;

    int spawnDir = 2;   //0,1,2,3->enemy moves right,up,left,down

    float slowTimeFloor = 0.2f;
    float normalTime = 1f;
    float timeFlowDelta = 0.01f;
    bool isSlowingTime = false;
    bool isRestoringTime = false;
    float timeDeltaIterDelayMS = 0.005f;
    float timeDeltaDramaDelayMS = 0.15f;

    bool alteringCameraSize = false;
    float cameraMinSize = 2,
        cameraMaxSize = 5f,
        cameraSizeTarget = 4,
        cameraSizeDelta = 0.02f;

    static float NO_CHANGE = 1234567;
    bool alteringScrollRate = false,
        alteringScrollHor = false,
        alteringScrollVer = false;
    float scrollSpeedMin = 0.001f,
        scrollSpeedMax = 0.007f,
        scrollCurrentHor,   
        scrollCurrentVer,
        scrollTargetHor,
        scrollTargetVer;

    bool gameRunning = false;
    bool messagePause = false;
    bool alteringLighting = false;

    bool spawnBeing = false;
    float minimumSpawningDelay = 0.3f,
        maximumSpawningDelay = 1.3f;

    float minWhaleSpawnSize = 3;

    bool spawnBossNext = false;
    bool bossAlive = false;
    bool clockingSpawnDelay = false;

    int numberOfBeingsSpawned = 0;

    int numberOfBeingsPerPhase = 20,
        numberOfBeingsPerPhaseMin = 20,
        numberOfBeingsPerPhaseMax = 40;

    int numberOfPhasesPerZoneCurrent = 0,
        numberOfPhasesPerZoneTarget = 4,
        numberOfPhasesPerZoneMin = 4,
        numberOfPhasesPerZoneMax = 6;

    int currentZone = 0;    //epi

    MenuRock startRock, quitRock;

    void Awake()
    {
        oceanBackground = (MeshRenderer)transform.GetChild(0).GetComponent<MeshRenderer>();
        spaceBackground = (MeshRenderer)transform.GetChild(1).GetComponent<MeshRenderer>();
        bubbles = (ParticleSystem)transform.GetChild(2).GetComponent<ParticleSystem>();

        fishPool = new List<GenericFish>();
        turtlePool = new List<Turtle>();
        octopusPool = new List<Octopus>();
        jellyfishPool = new List<Jellyfish>();
        blowfishPool = new List<Blowfish>();
        whalePool = new List<Whale>();
        wallJellyfishPool = new List<WallJellyfish>();
        miniAnglerFishPool = new List<MiniAnglerFish>();
        eelPool = new List<Eel>();

        saxophoneSharkPool = new List<SaxophoneShark>();
        poserShrimpPool = new List<PoserShrimp>();
        frontWhalePool = new List<FrontWhale>();
        mimicOctopusPool = new List<MimicOctopus>();
        unspeakableBeastPool = new List<UnspeakableBeast>();
        mothershipPool = new List<Mothership>();

        beamerPool = new List<StarshipBeamer>();
        discoPepperPool = new List<StarshipDiscoPepper>();

        activeBosses = new List<Enemy>();

        startRock = transform.GetChild(3).GetComponent<MenuRock>();
        quitRock = transform.GetChild(4).GetComponent<MenuRock>();
        mainLight = transform.GetChild(5).GetComponent<Light>();
    }

    void Start()
    {
        if (debug_short_levels)
        {
            numberOfBeingsPerPhase = 10;
            numberOfBeingsPerPhaseMin = 10;
            numberOfBeingsPerPhaseMax = 11;
        }
        if (debug_instaboss)
            spawnBossNext = true;
        nui = FindObjectOfType<NUI>();
        am = FindObjectOfType<AudioManager>();
        mainLight.intensity = zoneLightAmount[0];
        shrimp = FindObjectOfType<Shrimp>();
        if(debug_unparalyze_shrimp_during_test)
            shrimp.canMove = true;
        startRock.NoHealthFunc = BeginGame;
        quitRock.NoHealthFunc = ExitApplication;
        if (allowBeingsGoingDown)
            dirRandomMax = 4;
        else
            dirRandomMax = 3;
        if (debug_autostart)
        {
            BeginGame();
            RequestCameraSizeChange(Random.Range(3f, 3.5f));
            nui.KillLogoScreen();
            am.PlaySongsRandom();
        }
        else if (debug_roll_credits_immediately)
        {
            nui.KillLogoScreen();
            GameWon();
        }
        else
        {
            StartCoroutine(nui.RockLogoAndRevealStartScreen());
        }
    }

    public void BeginGame()
    {
        startRock.health_current = startRock.health_total;
        quitRock.health_current = quitRock.health_total;
        startRock.gameObject.SetActive(false);
        quitRock.gameObject.SetActive(false);
        gameRunning = true;
        shrimp.invinsible = false;
        currentZone = 0;
        spaceBeingsDestroyed = 0;
        numberOfBeingsPerPhase = Random.Range(numberOfBeingsPerPhaseMin, numberOfBeingsPerPhaseMax + 1);
        numberOfPhasesPerZoneCurrent = 0;
        numberOfPhasesPerZoneTarget = Random.Range(numberOfPhasesPerZoneMin, numberOfPhasesPerZoneMax + 1);
        if(debug_autostart)
            ChangeSpawnDirection(Random.Range(0, dirRandomMax));
        else
            RequestScrollChange(0.001f, NO_CHANGE);
        numberOfBeingsSpawned = 0;
        RequestMessageDisplay(zoneNames[currentZone]);
        if(debug_go_to_space)
            StartCoroutine(GoIntoSpace());
    }

    public void EndGame()
    {
        gameRunning = false;
        int size = activeBosses.Count;
        for(int i = size-1; i > -1; i--)
        {
            activeBosses[i].RecieveDamageMaximum();
        }
        if (inSpace)
        {
            inSpace = false;
            oceanBackground.gameObject.SetActive(true);
            spaceBackground.gameObject.SetActive(false);
            bubbles.Simulate(0, false, true);
            bubbles.Play();
        }
        startRock.gameObject.SetActive(true);
        quitRock.gameObject.SetActive(true);
        RequestCameraSizeChange(cameraMinSize);
        RequestScrollChange(0, 0);
        ChangeLighting(zoneLightAmount[0]);
        spawnDir = 2;
    }

    void GameWon()
    {
        StartCoroutine(nui.ProcessCredits());
        gameRunning = false;
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

	void Update()
    {
        if (gameRunning && !alteringCameraSize)
        {
            if (spawnBeing && !messagePause)
            {
                PonderSpawning();
            }else if (!clockingSpawnDelay)
            {
                StartCoroutine(ClockSpawnDelay());
            }
        }
    }

    public void RequestMessageDisplay(string message)
    {
        float f = nui.DisplayMessage(message);
        if (f > 0)
            StartCoroutine(ProcessMessagePause(f));
    }

    public IEnumerator SlowThenRestoreTime()
    {
        yield return StartCoroutine(SlowTime());
        yield return new WaitForSeconds(timeDeltaDramaDelayMS);
        yield return StartCoroutine(RestoreTime());
    }

    IEnumerator SlowTime()
    {
        isSlowingTime = true;
        isRestoringTime = false;
        while (isSlowingTime)
        {
            am.WarpTime(Time.timeScale = Mathf.Max(Time.timeScale - timeFlowDelta, slowTimeFloor));
            if (Time.timeScale == slowTimeFloor)
                isSlowingTime = false;
            else
                yield return new WaitForSeconds(timeDeltaIterDelayMS);
        }
    }

    IEnumerator RestoreTime()
    {
        isSlowingTime = false;
        isRestoringTime = true;
        while (isRestoringTime)
        {
            am.WarpTime(Time.timeScale = Mathf.Min(Time.timeScale + timeFlowDelta, normalTime));
            if (Time.timeScale == normalTime)
                isRestoringTime = false;
            else
                yield return new WaitForSeconds(timeDeltaIterDelayMS);
        }
    }

    IEnumerator ProcessMessagePause(float delay)
    {
        messagePause = true;
        yield return new WaitForSeconds(delay);
        messagePause = false;
    }

    IEnumerator ClockSpawnDelay()
    {
        clockingSpawnDelay = true;
        yield return new WaitForSeconds(Random.Range(minimumSpawningDelay, maximumSpawningDelay));
        clockingSpawnDelay = false;
        spawnBeing = true;
    }

    void FixedUpdate()
    {
        Vector2 offset = oceanBackground.material.GetTextureOffset("_MainTex");
        Vector2 offsetNew = new Vector2(offset.x + scrollCurrentHor, offset.y + scrollCurrentVer);
        if (oceanBackground.gameObject.activeInHierarchy)
        {
            offset = oceanBackground.material.GetTextureOffset("_MainTex");
            offsetNew = new Vector2(offset.x + scrollCurrentHor, offset.y + scrollCurrentVer);
            oceanBackground.material.SetTextureOffset("_MainTex", offsetNew);
        }
        if (spaceBackground.gameObject.activeInHierarchy)
        {
            offset = spaceBackground.material.GetTextureOffset("_MainTex");
            offsetNew = new Vector2(offset.x + scrollCurrentHor, offset.y + scrollCurrentVer);
            spaceBackground.material.SetTextureOffset("_MainTex", offsetNew);
        }
    }

    void PonderSpawning()
    {
        if (bossAlive == false)
        {
            if (spawnBossNext)
            {
                spawnBossNext = false;
                if (debug_autostart)
                {
                    RequestCameraSizeChange(zoneBossCameraSize[3]);
                    ChangeLighting(zoneLightAmount[3]);
                    SpawnMimicOctopus();

                }
                else//normal flow
                {
                    if (inSpace)
                        SpawnMothership();
                    else if (currentZone == 1)  //currentZone is bumped already, but spawn boss currentZone-1
                        SpawnSaxophoneShark();
                    else if (currentZone == 2)
                        SpawnFrontWhale();
                    else if (currentZone == 3)
                        SpawnPoserShrimp();
                    else if (currentZone == 4)
                        SpawnMimicOctopus();
                    else
                        SpawnUnspeakableBeast();  
                }
                bossAlive = true;
            }
            else
            {
                int rand = Random.Range(0, 100);
                if (inSpace)
                    SpawnSpaceBeings(rand);
                else if (currentZone == 0)
                    SpawnEpipelagicBeing(rand);
                else if (currentZone == 1)
                    SpawnMesopelagicBeing(rand);
                else if (currentZone == 2)
                    SpawnBathypelagicBeing(rand);
                else if (currentZone == 3)
                    SpawnAbyssopelagicBeing(rand);
                else if (currentZone == 4)
                    SpawnHadalpelagicBeing(rand);
                if (!inSpace)
                {
                    if (++numberOfBeingsSpawned == numberOfBeingsPerPhase)
                    {
                        if (++numberOfPhasesPerZoneCurrent == numberOfPhasesPerZoneTarget)
                        {
                            spawnBossNext = true;
                            RequestCameraSizeChange(zoneBossCameraSize[currentZone]);
                            RequestMessageDisplay(zoneBosses[currentZone]);
                            currentZone++;
                            numberOfPhasesPerZoneCurrent = 0;
                            numberOfPhasesPerZoneTarget = Random.Range(numberOfPhasesPerZoneMin, numberOfPhasesPerZoneMax + 1);
                        }
                        else
                        {
                            ChangeSpawnDirection(Random.Range(0, dirRandomMax));
                            RequestCameraSizeChange(Random.Range(3f, 3.5f));
                        }
                        numberOfBeingsSpawned = 0;
                        numberOfBeingsPerPhase = Random.Range(numberOfBeingsPerPhaseMin, numberOfBeingsPerPhaseMax + 1);

                    }
                }
                else
                //IN SPAAACCEee
                {
                    if (spaceBeingsDestroyed >= spaceBeingsToDestroy)
                    {
                        spawnBossNext = true;
                    }
                }
            }
        }
        spawnBeing = false;
    }

    void SpawnEpipelagicBeing(int rand)
    {
        if (rand < 45)
            SpawnGenericFish();
        else if (rand < 55)
            SpawnTurtleFish();
        else if (rand < 65)
            SpawnEel();
        else
            SpawnOctopus();
    }

    void SpawnMesopelagicBeing(int rand)
    {
        if (rand < 35)
            SpawnGenericFish();
        else if (rand < 55)
            SpawnTurtleFish();
        else if (rand < 85)
            SpawnEel();
        else if (rand < 90)
            SpawnOctopus();
        else if (rand < 94)
        {
            SpawnJellyfish();
        }
        else
        {
            if (WhaleCanSpawn())
                SpawnWhale();
            else
                SpawnBlowfish();
        }
    }

    void SpawnBathypelagicBeing(int rand)
    {
        if (rand < 25)
        {
            SpawnGenericFish();
        }
        else if (rand < 35)
            SpawnTurtleFish();
        else if (rand < 45)
            SpawnEel();
        else if (rand < 80)
        {
            SpawnOctopus();
        }
        else if (rand < 90)
        {
            SpawnJellyfish();
        }
        else
        {
            if (WhaleCanSpawn())
                SpawnWhale();
            else
                SpawnBlowfish();
        }
    }

    void SpawnAbyssopelagicBeing(int rand)
    {
        if (rand < 20)
            SpawnGenericFish();
        else if (rand < 75)
            SpawnOctopus();
        else if (rand < 85)
            SpawnBlowfish();
        else if (rand < 90)
            SpawnMiniAnglerFish();
        else
        {
            if (WhaleCanSpawn())
                SpawnWhale();
            else
                SpawnJellyfish();
        }
    }

    void SpawnHadalpelagicBeing(int rand)
    {
        if (rand < 60)
            SpawnOctopus();
        else if (rand < 75)
            SpawnJellyfish();
        else if (rand < 85)
        {
            if (WhaleCanSpawn())
                SpawnWhale();
            else
                SpawnBlowfish();
        }
        else
            SpawnMiniAnglerFish();
    }

    void SpawnSpaceBeings(int rand)
    {
        if (activeSpaceBeings == maxSpaceBeings)
            return;
        if (rand < 50)
            SpawnBeamer();
        else
            SpawnDiscoPepper();
    }

    bool WhaleCanSpawn()
    {
        return whaleAlive == 0 && 
            Camera.main.orthographicSize >= minWhaleSpawnSize &&
            numberOfPhasesPerZoneCurrent < numberOfPhasesPerZoneTarget - 1 &&
            UnityEngine.Random.Range(0, 2) == 0;
    }

    IEnumerator GoIntoSpace()
    {
        gameRunning = false;
        spawnDir = 3;
        RequestScrollChange(0, 0.02f);
        bubbles.Stop();
        yield return new WaitForSeconds(6);
        oceanBackground.gameObject.SetActive(false);
        spaceBackground.gameObject.SetActive(true);
        shrimp.transform.position = Vector3.zero;
        shrimp.invinsible = false;
        inSpace = true;
        gameRunning = true;
        ChangeLighting(zoneLightAmount[currentZone]);
        RequestCameraSizeChange(3.5f);
        RequestScrollChange(0, 0.007f);
        RequestMessageDisplay(zoneNames[5]);
    }

    //use NO_CHANGE to not alter a given var
    void RequestScrollChange(float hor, float ver)
    {
        if (alteringScrollRate)
            return;
        if (hor != NO_CHANGE)
        {
            alteringScrollRate = alteringScrollHor = true;
            scrollTargetHor = hor;
        }
        if(ver != NO_CHANGE)
        {
            alteringScrollRate = alteringScrollVer = true;
            scrollTargetVer = ver;
        }
        if (!alteringScrollRate)
            return;
        StartCoroutine(ProcessScroll());
    }

    IEnumerator ProcessScroll()
    {
        float horTemp = Mathf.Clamp(scrollCurrentHor, -0.25f, 0.25f),
            verTemp = Mathf.Clamp(scrollCurrentVer, -0.25f, 0.25f);
        for(float i = 0.10f; i < 1f; i+=0.025f)
        {
            if (!alteringScrollRate)
                break;
            if (alteringScrollHor)
            {
                scrollCurrentHor = Mathf.Lerp(horTemp, scrollTargetHor, i );
            }
            if (alteringScrollVer)
            {
                scrollCurrentVer = Mathf.Lerp(verTemp, scrollTargetVer, i );
            }
            yield return new WaitForSeconds(0.1f);
        }
        alteringScrollVer = alteringScrollHor = alteringScrollRate = false;
    }

    public Vector2 GetScrollInfo()
    {
        return new Vector2(scrollCurrentHor, scrollCurrentVer);
    }

    void RequestCameraSizeChange(float f)
    {
        cameraSizeTarget = Mathf.Clamp(f, cameraMinSize, cameraMaxSize);
        if (alteringCameraSize)
            return;
        alteringCameraSize = true;
        StartCoroutine(ProcessCameraSizeChange());
    }

    IEnumerator ProcessCameraSizeChange()
    {
        while (alteringCameraSize)
        {
            if(Camera.main.orthographicSize < cameraSizeTarget)
            {
                Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize + cameraSizeDelta, cameraSizeTarget);
            }else
            {
                Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - cameraSizeDelta, cameraSizeTarget);
            }
            if (Camera.main.orthographicSize == cameraSizeTarget)
                alteringCameraSize = false;
            yield return new WaitForSeconds(0.01f);
        }
    }

    void ChangeSpawnDirection(int i)
    {
        spawnDir = i;
        float whimOfTheGods = 0;
        if (Random.Range(0, 2) == 0)
            whimOfTheGods = NO_CHANGE;
        if (i == 0)//fish travel right
            RequestScrollChange(Random.Range(-scrollSpeedMax, -scrollSpeedMin), whimOfTheGods);
        else if (i == 2)
            RequestScrollChange(Random.Range(scrollSpeedMin, scrollSpeedMax), whimOfTheGods);
        else if(i==1)
            RequestScrollChange(whimOfTheGods, Random.Range(-scrollSpeedMin, -scrollSpeedMax));
        else
            RequestScrollChange(whimOfTheGods, Random.Range(scrollSpeedMin, scrollSpeedMax));
    }

    int TranslateToBeingDirValue()
    {
        if (spawnDir == 0 || spawnDir == 2)
            return spawnDir;
        if (spawnDir == 1)
            return 3;
        return 1;
    }

    Vector2 GetSpawnPosition()
    {
        Vector2 spawnPos;
        if (spawnDir == 0)
        {
            spawnPos = Camera.main.ViewportToWorldPoint(
                       new Vector3(0f, Random.Range(0f, 1f), 0f)
                    );
        }else if(spawnDir == 2)
        {
            spawnPos = Camera.main.ViewportToWorldPoint(
                   new Vector3(1f, Random.Range(0f, 1f), 0f)
                );
        }else if (spawnDir == 3)
        {
            spawnPos = Camera.main.ViewportToWorldPoint(
                   new Vector3(Random.Range(0f, 1f), 1f, 0f)
                );
        }else
        {
            spawnPos = Camera.main.ViewportToWorldPoint(
                   new Vector3(Random.Range(0f, 1f), 0f, 0f)
                );
        }
        return spawnPos;

    }

    Vector2 GetSpawnPositionWhale()
    {
        return GetSpawnPositionWhale(-1);
    }

    public Vector2 GetSpawnPositionWhale(int spawnDirOverride)
    {
        float diffPoint = 1.4f;
        Vector2 spawnPos;
        int spawnDir = spawnDirOverride;
        if (spawnDirOverride < 0 || spawnDirOverride > 3)
            spawnDir = this.spawnDir;
        if (spawnDir == 0)
        {
            spawnPos = Camera.main.ViewportToWorldPoint(
                       new Vector3(-diffPoint-0.4f, Random.Range(0f, 1f), -1f)
                    );
        }
        else if (spawnDir == 2)
        {
            spawnPos = Camera.main.ViewportToWorldPoint(
                   new Vector3(diffPoint, Random.Range(0f, 1f), -1f)
                );
        }
        else if (spawnDir == 3)
        {
            spawnPos = Camera.main.ViewportToWorldPoint(
                   new Vector3(Random.Range(0f, 1f), diffPoint, -1f)
                );
        }
        else
        {
            spawnPos = Camera.main.ViewportToWorldPoint(
                   new Vector3(Random.Range(0f, 1f), -diffPoint, -1f)
                );
        }
        return spawnPos;

    }


    void ChangeLighting(float intensity)
    {
        if (!alteringLighting)
            StartCoroutine(ProcessLightingChange(intensity));
    }

    IEnumerator ProcessLightingChange(float intensityGoal)
    {
        alteringLighting = true;
        float lightDelta = 0.01f;
        while (alteringLighting)
        {
            if (intensityGoal > mainLight.intensity)
            {
                mainLight.intensity = Mathf.Min(mainLight.intensity + lightDelta, intensityGoal);
            }
            else
            {
                mainLight.intensity = Mathf.Max(mainLight.intensity - lightDelta, intensityGoal);
            }
            if (intensityGoal == mainLight.intensity)
                alteringLighting = false;
            yield return new WaitForSeconds(0.01f);
        }
        alteringLighting = false;
    }

    GenericFish SpawnGenericFish()
    {
        fishAlive++;
        GenericFish fish = GetGenericFish();
        fish.transform.position = GetSpawnPosition();
        fish.gameObject.SetActive(true);
        fish.BeginMoving(TranslateToBeingDirValue());
        return fish;
    }

    Turtle SpawnTurtleFish()
    {
        Turtle fish = GetTurtle();
        fish.transform.position = GetSpawnPosition();
        fish.gameObject.SetActive(true);
        fish.BeginMoving(TranslateToBeingDirValue());
        return fish;
    }

    void SpawnJellyfish()
    {
        jellyfishAlive++;
        Jellyfish fish = GetJellyfish();
        fish.transform.position = GetSpawnPosition();
        fish.gameObject.SetActive(true);
        fish.BeginMoving(TranslateToBeingDirValue());
    }

    void SpawnWallJellyfish(Vector3 spawnPos, bool goRight)
    {
        WallJellyfish fish = GetWallJellyfish();
        fish.transform.position = spawnPos;
        fish.gameObject.SetActive(true);
        int dir = 0;
        if (!goRight)
            dir = 2;
        fish.BeginMoving(dir);

    }

    public void SpawnJellyfishWall(float xpos, int missingSpot)
    {
        int wallMax = 5;
        if (missingSpot < 0 || missingSpot > wallMax)
            missingSpot = Random.Range(0, wallMax+1);

        float ydelta = (100 / 6) / 100f;
        Vector3 spawnPoint;
        for(float i = ydelta, m = 0; i < 1; i=i+ydelta, m++)
        {
            if(m!=missingSpot)
            {
                spawnPoint = Camera.main.ViewportToWorldPoint(
                    new Vector3(0, i, 0)
                 );
                spawnPoint = new Vector3(xpos, spawnPoint.y, 0);
                SpawnWallJellyfish(spawnPoint, xpos < 0);
            }
        }
    }

    public IEnumerator MimicOctopusSpawnGenericFishSchool()
    {
        bool octopusAssigned = false;
        spawnDir = 0;
        Enemy e;
        if (Random.Range(0, 2) == 0)
            spawnDir = 2;
        for(int i = 0; i < 16; i++)
        {
            e = SpawnGenericFish();
            if (!octopusAssigned)
            {
                if (i > 5)
                {
                    e.OnRecieveDamage = e.DisguiseDiscovered;
                    octopusAssigned = true;
                }else if(i > 12)
                {
                    e.OnRecieveDamage = e.DisguiseDiscovered;
                    octopusAssigned = true;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    Octopus SpawnOctopus()
    {
        octopusAlive++;
        Vector2 spawnPos = Camera.main.ViewportToWorldPoint(
                   new Vector3(1f, Random.Range(0f, 1f), 0f)
                );
        Octopus fish = GetOctopus();
        fish.gameObject.SetActive(true);
        fish.transform.position = GetSpawnPosition();
        fish.BeginMoving(TranslateToBeingDirValue());
        return fish;
    }

    public IEnumerator MimicOctopusSpawnOctopiSchool()
    {
        bool octopusAssigned = false;
        spawnDir = 0;
        Enemy e;
        if (Random.Range(0, 2) == 0)
            spawnDir = 2;
        for (int i = 0; i < 16; i++)
        {
            e = SpawnOctopus();
            if (!octopusAssigned)
            {
                if (i > 5)
                {
                    e.OnRecieveDamage = e.DisguiseDiscovered;
                    octopusAssigned = true;
                }
                else if (i > 12)
                {
                    e.OnRecieveDamage = e.DisguiseDiscovered;
                    octopusAssigned = true;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void SpawnBlowfish()
    {
        blowfishAlive++;
        Blowfish fish = GetBlowfish();
        fish.gameObject.SetActive(true);
        fish.transform.position = GetSpawnPosition();
        fish.BeginMoving(TranslateToBeingDirValue());
    }

    void SpawnWhale()
    {
        whaleAlive++;
        Whale fish = GetWhale();
        fish.gameObject.SetActive(true);
        fish.transform.position = GetSpawnPositionWhale();
        fish.BeginMoving(TranslateToBeingDirValue());
    }

    void SpawnMiniAnglerFish()
    {
        MiniAnglerFish fish = GetMiniAnglerFish();
        fish.gameObject.SetActive(true);
        fish.transform.position = GetSpawnPosition();
        fish.BeginMoving(TranslateToBeingDirValue());
    }

    public void SpawnMiniAnglerFish(Vector3 spawnPos)
    {
        MiniAnglerFish fish = GetMiniAnglerFish();
        fish.gameObject.SetActive(true);
        fish.transform.position = spawnPos;
        fish.BeginMoving(TranslateToBeingDirValue());
    }

    public void SpawnEel()
    {
        Eel fish = GetEel();
        fish.gameObject.SetActive(true);
        fish.transform.position = GetSpawnPosition();
        fish.BeginMoving(TranslateToBeingDirValue());
    }

    void SpawnSaxophoneShark()
    {
        SaxophoneShark s = GetSaxophoneShark();
        activeBosses.Add(s);
        s.gameObject.SetActive(true);
        s.transform.position = GetSpawnPosition();
        s.BeginMoving(TranslateToBeingDirValue());
    }

    void SpawnPoserShrimp()
    {
        PoserShrimp s = GetPoserShrimp();
        activeBosses.Add(s);
        s.gameObject.SetActive(true);
        s.transform.position = GetSpawnPosition();
        s.BeginMoving(TranslateToBeingDirValue());
    }

    void SpawnFrontWhale()
    {
        FrontWhale s = GetFrontWhale();
        activeBosses.Add(s);
        s.gameObject.SetActive(true);
        //handles own transform placement
        s.BeginMoving(TranslateToBeingDirValue());
    }

    void SpawnMimicOctopus()
    {
        MimicOctopus s = GetMimicOctopus();
        activeBosses.Add(s);
        s.gameObject.SetActive(true);
        //handles own transform placement
        s.BeginMoving(TranslateToBeingDirValue());
    }

    void SpawnUnspeakableBeast()
    {
        UnspeakableBeast s = GetUnspeakableBeast();
        activeBosses.Add(s);
        s.gameObject.SetActive(true);
        s.transform.position = GetSpawnPosition();
        s.BeginMoving(TranslateToBeingDirValue());
    }

    void SpawnMothership()
    {
        Mothership s = GetMothership();
        activeBosses.Add(s);
        s.gameObject.SetActive(true);
        spawnDir = 3;
        s.transform.position = GetSpawnPosition();
        s.transform.position = new Vector3(0, s.transform.position.y, -4);
        s.BeginMoving(TranslateToBeingDirValue());
    }

    void SpawnBeamer()
    {
        ++activeSpaceBeings;
        StarshipBeamer s = GetBeamer();
        activeBosses.Add(s);
        s.gameObject.SetActive(true);
        spawnDir = 3;
        s.transform.position = GetSpawnPosition();
        s.BeginMoving(TranslateToBeingDirValue());
    }

    void SpawnDiscoPepper()
    {
        ++activeSpaceBeings;
        StarshipDiscoPepper s = GetDiscoPepper();
        activeBosses.Add(s);
        s.gameObject.SetActive(true);
        spawnDir = 3;
        s.transform.position = GetSpawnPosition();
        s.BeginMoving(TranslateToBeingDirValue());
    }

    WallJellyfish GetWallJellyfish()
    {
        WallJellyfish f = null;
        if (wallJellyfishPool.Count > 0)
        {
            f = wallJellyfishPool[0];
            wallJellyfishPool.RemoveAt(0);
        }
        else
        {
            return (WallJellyfish)Instantiate(Resources.Load("Prefabs\\Enemies\\WallJellyfish", typeof(WallJellyfish)));
        }
        return f;
    }

    GenericFish GetGenericFish()
    {
        GenericFish f = null;
        if ( fishPool.Count > 0)
        {
            f = fishPool[0];
            fishPool.RemoveAt(0);
        }
        else
        {
            return (GenericFish)Instantiate(Resources.Load("Prefabs\\Enemies\\GenericFish", typeof(GenericFish)));
        }
        return f;
    }

    public void ReturnGenericFishToPool(GenericFish g)
    {
        fishAlive--;
        fishPool.Add(g);
        g.gameObject.SetActive(false);
    }

    Turtle GetTurtle()
    {
        Turtle f = null;
        if (turtlePool.Count > 0)
        {
            f = turtlePool[0];
            turtlePool.RemoveAt(0);
        }
        else
        {
            return (Turtle)Instantiate(Resources.Load("Prefabs\\Enemies\\Turtle", typeof(Turtle)));
        }
        return f;
    }

    public void ReturnTurtleToPool(Turtle g)
    {
        turtlePool.Add(g);
        g.gameObject.SetActive(false);
    }


    Jellyfish GetJellyfish()
    {
        Jellyfish f = null;
        if (jellyfishPool.Count > 0)
        {
            f = jellyfishPool[0];
            jellyfishPool.RemoveAt(0);
        }
        else
        {
            return (Jellyfish)Instantiate(Resources.Load("Prefabs\\Enemies\\Jellyfish", typeof(Jellyfish)));
        }
        return f;
    }

    public void ReturnJellyfishToPool(Jellyfish g)
    {
        jellyfishAlive--;
        jellyfishPool.Add(g);
        g.gameObject.SetActive(false);
    }

    Octopus GetOctopus()
    {
        Octopus f = null;
        if (octopusPool.Count > 0)
        {
            f = octopusPool[0];
            octopusPool.RemoveAt(0);
        }
        else
        {
            return (Octopus)Instantiate(Resources.Load("Prefabs\\Enemies\\Octopus", typeof(Octopus)));
        }
        return f;
    }

    public void ReturnOctopusToPool(Octopus g)
    {
        octopusAlive--;
        octopusPool.Add(g);
        g.gameObject.SetActive(false);
    }

    Blowfish GetBlowfish()
    {
        Blowfish f = null;
        if (blowfishPool.Count > 0)
        {
            f = blowfishPool[0];
            blowfishPool.RemoveAt(0);
        }
        else
        {
            return (Blowfish)Instantiate(Resources.Load("Prefabs\\Enemies\\Blowfish", typeof(Blowfish)));
        }
        return f;
    }

    public void ReturnBlowfishToPool(Blowfish g)
    {
        blowfishAlive--;
        blowfishPool.Add(g);
        g.gameObject.SetActive(false);
    }

    Whale GetWhale()
    {
        Whale f = null;
        if (whalePool.Count > 0)
        {
            f = whalePool[0];
            whalePool.RemoveAt(0);
        }
        else
        {
            return (Whale)Instantiate(Resources.Load("Prefabs\\Enemies\\Whale", typeof(Whale)));
        }
        return f;
    }

    public void ReturnWhaleToPool(Whale g)
    {
        whaleAlive--;
        whalePool.Add(g);
        g.gameObject.SetActive(false);
    }

    MiniAnglerFish GetMiniAnglerFish()
    {
        MiniAnglerFish f = null;
        if (miniAnglerFishPool.Count > 0)
        {
            f = miniAnglerFishPool[0];
            miniAnglerFishPool.RemoveAt(0);
        }
        else
        {
            return (MiniAnglerFish)Instantiate(Resources.Load("Prefabs\\Enemies\\MiniAnglerFish", typeof(MiniAnglerFish)));
        }
        return f;
    }

    public void ReturnMiniAnglerFishToPool(MiniAnglerFish g)
    {
        miniAnglerFishPool.Add(g);
        g.gameObject.SetActive(false);
    }

    Eel GetEel()
    {
        Eel f = null;
        if (eelPool.Count > 0)
        {
            f = eelPool[0];
            eelPool.RemoveAt(0);
        }
        else
        {
            return (Eel)Instantiate(Resources.Load("Prefabs\\Enemies\\Eel", typeof(Eel)));
        }
        return f;
    }

    public void ReturnEelToPool(Eel g)
    {
        eelPool.Add(g);
        g.gameObject.SetActive(false);
    }

    public void ReturnWallJellyfishToPool(WallJellyfish g)
    {
        wallJellyfishPool.Add(g);
        g.gameObject.SetActive(false);
    }

    SaxophoneShark GetSaxophoneShark()
    {
        SaxophoneShark f = null;
        if (saxophoneSharkPool.Count > 0)
        {
            f = saxophoneSharkPool[0];
            saxophoneSharkPool.RemoveAt(0);
        }
        else
        {
            return (SaxophoneShark)Instantiate(Resources.Load("Prefabs\\Enemies\\SaxophoneShark", typeof(SaxophoneShark)));
        }
        return f;
    }

    public void ReturnSaxophoneSharkToPool(SaxophoneShark g)
    {
        activeBosses.Remove(g);
        //saxophoneSharkPool.Add(g);
        g.gameObject.SetActive(false);
        bossAlive = false;
        if (gameRunning)
        {
            ChangeSpawnDirection(Random.Range(0, dirRandomMax));
            RequestCameraSizeChange(Random.Range(cameraMinSize, 3.5f));
            RequestMessageDisplay(zoneNames[currentZone]);
            ChangeLighting(zoneLightAmount[currentZone]);
        }
    }

    PoserShrimp GetPoserShrimp()
    {
        PoserShrimp f = null;
        if (poserShrimpPool.Count > 0)
        {
            f = poserShrimpPool[0];
            poserShrimpPool.RemoveAt(0);
        }
        else
        {
            return (PoserShrimp)Instantiate(Resources.Load("Prefabs\\Enemies\\PoserShrimp", typeof(PoserShrimp)));
        }
        return f;
    }

    public void ReturnPoserShrimpToPool(PoserShrimp g)
    {
        activeBosses.Remove(g);
        poserShrimpPool.Add(g);
        g.gameObject.SetActive(false);
        bossAlive = false;
        if (gameRunning)
        {
            ChangeSpawnDirection(Random.Range(0, dirRandomMax));
            RequestCameraSizeChange(Random.Range(cameraMinSize, 3.5f));
            RequestMessageDisplay(zoneNames[currentZone]);
            ChangeLighting(zoneLightAmount[currentZone]);
        }
    }

    FrontWhale GetFrontWhale()
    {
        FrontWhale f = null;
        if (frontWhalePool.Count > 0)
        {
            f = frontWhalePool[0];
            frontWhalePool.RemoveAt(0);
        }
        else
        {
            return (FrontWhale)Instantiate(Resources.Load("Prefabs\\Enemies\\FrontWhale", typeof(FrontWhale)));
        }
        return f;
    }

    public void ReturnFrontWhaleToPool(FrontWhale g)
    {
        activeBosses.Remove(g);
        //frontWhalePool.Add(g);
        g.gameObject.SetActive(false);
        bossAlive = false;
        if (gameRunning)
        {
            ChangeSpawnDirection(Random.Range(0, dirRandomMax));
            RequestCameraSizeChange(Random.Range(cameraMinSize, 3.5f));
            RequestMessageDisplay(zoneNames[currentZone]);
            ChangeLighting(zoneLightAmount[currentZone]);
        }
    }

    MimicOctopus GetMimicOctopus()
    {
        MimicOctopus f = null;
        if (mimicOctopusPool.Count > 0)
        {
            f = mimicOctopusPool[0];
            mimicOctopusPool.RemoveAt(0);
        }
        else
        {
            return (MimicOctopus)Instantiate(Resources.Load("Prefabs\\Enemies\\MimicOctopus", typeof(MimicOctopus)));
        }
        return f;
    }

    public void ReturnMimicOctopusToPool(MimicOctopus g)
    {
        activeBosses.Remove(g);
        //mimicOctopusPool.Add(g);
        g.gameObject.SetActive(false);
        bossAlive = false;
        if (gameRunning)
        {
            ChangeSpawnDirection(Random.Range(0, dirRandomMax));
            RequestCameraSizeChange(Random.Range(cameraMinSize, 3.5f));
            RequestMessageDisplay(zoneNames[currentZone]);
            ChangeLighting(zoneLightAmount[currentZone]);
        }
    }

    UnspeakableBeast GetUnspeakableBeast()
    {
        UnspeakableBeast f = null;
        if (unspeakableBeastPool.Count > 0)
        {
            f = unspeakableBeastPool[0];
            unspeakableBeastPool.RemoveAt(0);
        }
        else
        {   
            return (UnspeakableBeast)Instantiate(Resources.Load("Prefabs\\Enemies\\AnglerFish", typeof(UnspeakableBeast)));
        }
        return f;
    }

    public void ReturnUnspeakableBeastToPool(UnspeakableBeast g)
    {
        activeBosses.Remove(g);
        //unspeakableBeastPool.Add(g);
        g.gameObject.SetActive(false);
        bossAlive = false;
        if (gameRunning)
        {
            StartCoroutine(GoIntoSpace());
        }
    }

    Mothership GetMothership()
    {
        Mothership f = null;
        if (mothershipPool.Count > 0)
        {
            f = mothershipPool[0];
            mothershipPool.RemoveAt(0);
        }
        else
        {
            return (Mothership)Instantiate(Resources.Load("Prefabs\\Enemies\\Mothership", typeof(Mothership)));
        }
        return f;
    }

    public void ReturnMothershipToPool(Mothership g)
    {
        activeBosses.Remove(g);
        //mothershipPool.Add(g);
        g.gameObject.SetActive(false);
        bossAlive = false;
        if (gameRunning)
        {
            GameWon();
        }
    }

    StarshipBeamer GetBeamer()
    {
        StarshipBeamer f = null;
        if (beamerPool.Count > 0)
        {
            f = beamerPool[0];
            beamerPool.RemoveAt(0);
        }
        else
        {
            return (StarshipBeamer)Instantiate(Resources.Load("Prefabs\\Enemies\\Beamer", typeof(StarshipBeamer)));
        }
        return f;
    }

    public void ReturnBeamerToPool(StarshipBeamer g)
    {
        --activeSpaceBeings;
        spaceBeingsDestroyed++;
        activeBosses.Remove(g);
        beamerPool.Add(g);
        g.gameObject.SetActive(false);
    }

    public StarshipDiscoPepper GetDiscoPepper()
    {
        StarshipDiscoPepper f = null;
        if (discoPepperPool.Count > 0)
        {
            f = discoPepperPool[0];
            discoPepperPool.RemoveAt(0);
        }
        else
        {
            return (StarshipDiscoPepper)Instantiate(Resources.Load("Prefabs\\Enemies\\DiscoPepper", typeof(StarshipDiscoPepper)));
        }
        return f;
    }

    public void ReturnDiscoPepperToPool(StarshipDiscoPepper g)
    {
        --activeSpaceBeings;
        spaceBeingsDestroyed++;
        activeBosses.Remove(g);
        discoPepperPool.Add(g);
        g.gameObject.SetActive(false);
    }
}
