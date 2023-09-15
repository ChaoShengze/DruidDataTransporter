配置文件说明：

{
    # 当前下载的进度，程序会自动更新以支持文件粒度的断点续传。注意，未开始任务为0，完成一个下载才+1.
    "CurrBlock": 42,
    # 每个Block文件存放的条数
    "BlockSize": 10000,
    # 存放路径，切片文件会自动以xxx.json.000001的样式存储
    "SavePath": "Blocks\\data.json",
    # Druid的RestApi地址，记得末尾是sql！！！
    "DruidUrl": "http://xxx:8888/druid/v2/sql",
    # 如果没有设置用户名密码清留空
    "DruidUsr": "",
    # 如果没有设置用户名密码清留空
    "DruidPwd": "",
    # 查询数据语句，不需要带LIMIT，程序会自动更新补全
    "DruidQuery": "",
    # 总量查询语句，记住结果一定要使用 AS Count，否则会无法正常解析
    "DruidCountQuery": "SELECT COUNT(*) AS \"Count\" FROM \"database\" GROUP BY ()",
    # 查询的超时设置，数据量较大时需要设置较大值保证查询能够完成，单位：分钟
    "QueryTimeout": 10
}