# baddies

ToDo list, in more or less priority order:

- camera control
    pan/drag
    rotate
    zoom
- player* AI (we need to discuss what it should do but running away from enemies not just standing there is a good start :)
    first of all, "see" enemies
    move away from enemies to stay alive
    shoot enemies
    melee attack?
    have some goal
- click to spawn zombie**
- find or make graphics: zombie, player, level
- audio
- game rules (gameplay.cs):
    detect player reaching their goal, load next level
    detect player dying, game over(yaaay!)
    detect player reaching the final goal, victory(boo!)
    some restriction of where/when we can/cannot spawn zombies
- end level goal object ("exit") for player* to reach
- "powerups" for player
- levels (design and build)


*from our point of view, the enemy; let's call him HERO instead
**only calling them zombie for now, depending on what visuals/setting we end up with might be an orc, german, monster, robot, whatever. Our grunts.
