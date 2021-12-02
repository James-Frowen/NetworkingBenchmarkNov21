# Network Benchmark November 2021

Testing differnet metrics for Mirage vs Mirror.

## The Benchmark
- 1 scene
- 100m x 100m
- 10 monsters connected player
- Players move to random position
- If player collides with monster it will deal damage to monster
- if monster reaches 0 hp it will die
- Server will respawn monsters each frame to keep the correct ammount
- Both Server and Client have object pooling for the monsters

## VPS Settings
google cloud
- e2-small


## key
- noAoi - no visibility scripts
- Aoi1 - proximity checker
- Aoi2 - mirror's new IM system
- Aoi4 - new optimized grid
- NT1 - old NT
- NT2 - mirror's new NT NT2k

## results

| Build name                                   | Library | AOI                              | NT version | CPU  | Bandwidth | Memory |
|----------------------------------------------|---------|----------------------------------|------------|------|-----------|--------|
| benchmarkNov21_noAoi_NT1_server_linux        | Mirage  | None                             | Legacy NT  |      |           |        |
| benchmarkNov21_mirror_noAoi_NT2_server_linux | Mirror  | None                             | NT2k       |      |           |        |
| benchmarkNov21_Aoi1_NT1_server_linux         | Mirage  | ProximityChecker                 | Legacy NT  |      |           |        |
| benchmarkNov21_mirror_Aoi2_NT2_server_linux  | Mirror  | SpatialHashingInterestManagement | NT2k       |      |           |        |
| benchmarkNov21_Aoi4_NT1_server_linux         | Mirage  | SpatialHashSystem                | Legacy NT  |      |           |        |


## Raw

### benchmarkNov21_noAoi_NT1_server_linux


