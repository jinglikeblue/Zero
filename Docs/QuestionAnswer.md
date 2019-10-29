# Q&A

- <font color=#488AF3>为什么Asset/@Scripts/Zero中的内容不放到Asset/Zero/Scripts中？  </font>

> 首先Asset/@Scripts/Zero目录中的代码目标是可以打包为DLL，利用ILRuntime框架进行热更的。那么其中的代码AView是Zero中视图对象的基类，如果放到Asset/Zero/Scripts中会产生「跨域继承」以及复杂的「泛型调用」（参看ILRutnime的相关介绍）的问题。
