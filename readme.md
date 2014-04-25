# SquareCubed Engine #
SquareCubed Engine is an open source 2D game engine. Its focus is on
multiplayer and modding. It is available for free via github under
GPLv3 for non-commercial. Contact EdgeWorks Games(TM) for commercial
licensing.

## Blink ##
Blink is the game this engine was started for. Currently the source
for Blink and this engine are exactly the same thing. Assets for blink
are in this repository as well.

## Modding ##
If you want to make a mod for this engine or games built on it, you
do not need to download the entire engine source and build it. We
are working on a separate guide for modding.

## Getting Started ##
To use this engine you need to get and set up a few dependencies.
While we tried to use NuGet as much as possible, not everything was
available through it.

1. Download and install the [Code Contracts](http://visualstudiogallery.msdn.microsoft.com/1ec7db13-3363-46c9-851f-1ce455f66970)
   extension for Visual Studio.
2. Download [Coherent UI .NET](http://coherent-labs.com/download-dotnet/).
   Evaluation version works.
3. Place Coherent UI in the root solution directory under the folder "CoherentUI".
4. Download [Lidgren.Network](https://code.google.com/p/lidgren-network-gen3/).
5. Place Lidgren.Network in the root solution directory under the folder "Lidgren.Network".
6. Set Lidgren.Network code contract settings to "Custom Parameter Validation" and "Build".
7. [Allow NuGet to download missing packages during build.](http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages)
8. _(Optional) Install [Web Essentials](http://vswebessentials.com/) extension for Visual Studio._
9. Build the engine from Visual Studio.

## Branches ##
This project uses [Git Flow](http://nvie.com/posts/a-successful-git-branching-model/).
If you are looking for the latest develop version, look at the develop branch.
The latest stable version is the master branch.

## Code License ##
This license covers the all the code in this project (.cs, .cshtml, .html, .js, .coffee, .scss, etc...)
unless specifically stated otherwise in the file.
<div align="center">
    <p>
        SquareCubed Engine - A 2D multiplayer modding focused game engine.<br />
        Copyright (C) 2014  EdgeWorks Games(TM)
    </p>
    <p>
        This program is free software: you can redistribute it and/or modify
        it under the terms of the GNU General Public License as published by
        the Free Software Foundation, either version 3 of the License, or
        any later version.
    </p>
    <p>
        This program is distributed in the hope that it will be useful,
        but WITHOUT ANY WARRANTY; without even the implied warranty of
        MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
        GNU General Public License for more details.
    </p>
    <p>
        You should have received a copy of the GNU General Public License
        along with this program. If not, see http://www.gnu.org/licenses/.
        For contacting EdgeWorks Games directly, see http://edgeworksgames.com/.
    </p>
</div>

## Non-Code License ##
This license covers all the non-code in this project. (.png, .psd, etc...)
<div align="center">
    <a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/">
        <img alt="Creative Commons License"
             style="border-width:0"
             src="http://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png" />
    </a><br />
    This work by <a xmlns:cc="http://creativecommons.org/ns#" href="http://edgeworksgames.com/" property="cc:attributionName" rel="cc:attributionURL">EdgeWorks Games(TM)</a> is licensed under a <a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/">Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License</a>.
</div>