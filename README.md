# baddies

ToDo list, in more or less priority order:

- [x] camera control\
    pan/drag\
    rotate\
    zoom
- [x] player* AI (we need to discuss what it should do but running away from enemies not just standing there is a good start :)\
    first of all, "see" enemies\
    move away from enemies to stay alive\
    shoot enemies\
    melee attack?\
    have some goal
- [x] click to spawn zombie**
- [x] find or make graphics: zombie, player, weapons, level building blocks (models and textures)
- audio
- game rules (gameplay.cs):\
    detect player reaching their goal, load next level\
    detect player dying, game over(yaaay!)\
    detect player reaching the final goal, victory(boo!)\
    some restriction of where/when we can/cannot spawn zombies
- [ ] end level goal object ("exit") for player* to reach
- [ ]"powerups" for player
- [ ] levels (design and build)


*from our point of view, the enemy; let's call him HERO instead

**only calling them zombie for now, depending on what visuals/setting we end up with might be an orc, german, monster, robot, whatever. Our grunts.


TOMORROW:
- [x] build an "actual" level with proper graphics, navmesh, and the working prefabs,
- [x] enable/fix spawning with mouse
- [ ] make other types of pickup for the player, at least...\
      EXIT (if the player reaches it, monsters failed, next level)\
      ...but potentially also weapon upgrades, whatever we can
- [ ] !! update the UI and the game rules / announcements\
    - [ ] player reaching the EXIT -> load next level
    - [ ] player dies -> restart level
- [ ] add some limits on spawning
    - [ ] number of monsters,
    - [x] out of sight of player
- [ ] make a few more levels
