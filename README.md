# New feature added:
Now the planet have water and can be populate:
![](https://i.gyazo.com/0e9afa84b7d5c65a1bc8ffcbf31b4211.png)

# Procedural-planet-generation (Unity Project)
Generate planets proceduraly.
[(Generate)](https://thehunterjp.itch.io/planetary-generation-prototype)
# Low Poly style:
<a href="https://imgflip.com/gif/2crlvi"><img src="https://i.imgflip.com/2crlvi.gif" title="made at imgflip.com"/></a>

# Terrace style:
<a href="https://imgflip.com/gif/2crlff"><img src="https://i.imgflip.com/2crlff.gif" title="made at imgflip.com"/></a>
![](https://i.gyazo.com/e15d76e0091e84ada206e3a14787739b.gif)


# How to use?
#### We only need to **import 5 files**: 
 'Planet.cs' , 'PlanetData.cs' , 'Polygon.cs' , 'ColorHeight.cs' , 'GenerationData.cs'.

#### GenerationData:
 This scriptable object stores the information that will then be used to get the height of the map vertices (it uses perlin noise).

 ![](https://i.gyazo.com/18a6e7d72de6d9383ae1c72503ba8d45.png)

#### PlanetData:
 This scriptable object stores the information that will then be used to create a planet.

 ![](https://i.gyazo.com/fc828f1192b56b41dce73e9fcf1df70c.png)

#### Generate the planets:
 To add a planet to the list of planets that will be created, use the functtion:
> Planet.AddPlanetToQueu();

To start the thread that will calculate the data for the mesh sphere generation and modification of the landscape (will only be executed after StartDataQueu has ended):
> Planet.StartDataQueu();

Instantiate the planet into the scene:
>Planet.InstantiateIntoWorld();

Wait until the planet data have been compute. Then load the chunks in range to the position.
>Planet.HideAndShow(Vector3 position);

Both InstantiateIntoWorld and HideAndShow can be used at the same time. In that case
InstantiateIntoWorld will generate the planet until it's fully generated and the will hide all the
terrain that is not in range of the position.

The end result should look like this:
![](https://i.gyazo.com/f6c185f8c376b7a620202f5381a32ffe.png)
