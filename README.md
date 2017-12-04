# UnityTools
记录一些有用的插件及脚本<br/>
1.加密解密文件，采用AES加密文件
<br/>
2.AssetBundle打包<br/>
3.压缩字符串，引用Ionic.BZip2.dll(unity提供该dll，平台通用)<br/>
4.FPS简单显示<br/>

# 2016年9月23日<br/>
1.支付脚本，使用Unity支付插件，封装信息，提供接口<br/>
2.增加文件署名，<br/>将UnityTools\81-C# Script-NewBehaviourScript.cs替换位置：untiy安装路径..\Editor\Data\Resources\ScriptTemplates\81-C# Script-NewBehaviourScript.cs

# 2017年7月7日<br/>
1.增加生成9x9 sudoku的随机解法，采用递归方法
  可延伸出：a.求解残局，b.存在的数独是否有解，c.设置数独难度，d.生成非9x9的数独解法
  
# 2017年12月4日<br/>
1.Custom Font生成<br/>
  这两天看到别人写的艺术数字，挺好的，也想写一下。这个想法以前就有过，但是却没认真对待，都知难而退。这次看了一个小伙伴做好的字体，反推了一下代码生成。<br/>
  难度主要在Character Rects中的各种设置。<br/>
 遇到的问题: a.做好以后，长度是足够的，但是数字却会相互覆盖，缺少了Advance的设置<br/>
            b.Vert的Height 其中是负数<br/>
            c.UV及Vert各种Obsolete<br/>
            d.Font的Ascii start offset 设置后，Element的Index可以从0开始，读取方式是 offset+index = ascii<br/>
2017年12月6日前把代码更新下。<br/>
前提: 图片可以经过TexturePacker处理，也可以直接是一张图，然后在Unity里面进行分割。分割的图片以 A_number保存，具体可以修改代码来完成具体需求。代码只满足数字0-9的处理<br/>


# 2017年12月5日<br/>
1.更新一个三色渐变UGUI Text脚本
