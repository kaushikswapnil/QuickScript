App
{
    void init(int num_objects);
    void cleanup();
    bool run();
    void render();
    void update_objects();
    bool debug_log();

    *Tile mTiles[30]; *
    int mObjSize;

    bool mDebugLog; 
}

