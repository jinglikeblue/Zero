- 为什么Asset/ZeroIL/Zero中的内容不放到Asset/Zero中？  
> 首先Asset/ZeroIL目录中的代码目标是可以打包为DLL，利用ILRuntime框架进行热更的。那么其中的代码AView是Zero中视图对象的基类，如果放到Asset/Zero中会产生「跨域继承」以及复杂的「泛型调用」（参看ILRutnime的相关介绍）的问题。

- DLL库项目如何快速包含所有拷贝的代码？
> 编辑csproj文件，使用通配符来包含代码即可  

```
<ItemGroup>  
    <Compile Include="**\*.cs" />  
</ItemGroup>
```
