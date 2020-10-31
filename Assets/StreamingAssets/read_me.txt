### 1.解压package.zip
>在我们打包发布APP的时候，有些资源希望内嵌在APP中，那么我们可以选择将这些资源压缩为「package.zip」（**注意名字、大小写必须一致**），并放到StreamingAssets目录下，这样打包的时候便会和APP一起发布。

程序启动的时候，会检测如果是第一次安装程序，且存在package.zip，便会将其解压出来。解压后的资源使用方式和热更资源的使用方式一致，参考 **「资源管理解决方案」**

```
注意：package.zip中的内容会解压到项目的可读写目录「ZeroConst.PERSISTENT_DATA_PATH」中，如果希望初始化时的资源能够正确匹配，请严格检测压缩文件中的目录路径和下载资源的目录路径一致。可以参考常量「ZeroConst.WWW_RES_PERSISTENT_DATA_PATH」
```