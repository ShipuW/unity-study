using UnityEngine;
using System.Collections;

public class AddLabel : MonoBehaviour {

	
	public UITextList textList = null;
	//
	private Menu menu = null;
	private bool m_bInit = false;
	// Use this for initialization
	void Start () {
		
		menu = this.GetComponent<Menu>();		
		if (textList == null) {
			Debug.LogError("There's not any Component:'UITextList'.",this);
			return;
		}

		//Sun or Introduction
		if (Application.loadedLevel == 0 || Application.loadedLevel == 4) {
			if (menu.ID == 1) {
				textList.Add ("    太阳是太阳系中唯一的恒星和会发光天");
				textList.Add("体，是太阳系的中心天体，太阳系质量的99.86%都集中在太阳。" +
				             "太阳系中的八大行星、小行星、流星、彗星、外海王星天体以及星际尘埃等，都围绕着太阳运行（公转）。而太阳则围绕着银河系的中心运行，也就是公转。");
				textList.Add("    太阳是位于太阳系中心的恒星，\n它几乎是热等离子体与磁场交织着的一个理想球体。" +
				             "太阳直径大约是1392000（1.392×10^6）公里，相当于地球直径的109倍；体积大约是地球的130万倍；其质量大约是2×10^30千克（地球的330000倍）。" +
				             "从化学组成来看，现在太阳质量的大约四分之三是氢，剩下的几乎都是氦，包括氧、碳、氖、铁和其他的重元素质量少于2%。");
				textList.Add("太阳目前正在穿越银河系内部边缘猎户臂的本地泡区中的本星际云。在距离地球17光年的距离内有50颗最邻近的恒星系（最接近的一颗是红矮星，被称为比邻星）。");
				textList.Add("太阳是一颗黄矮星（光谱为G2V），黄矮星的寿命大致为100亿年，目前太阳大约45.7亿岁。" +
				             "在大约50至60亿年之后，太阳内部的氢元素几乎会全部消耗尽，太阳的核心将发生坍缩，导致温度上升，" +
				             "这一过程将一直持续到太阳开始把氦元素聚变成碳元素。虽然氦聚变产生的能量比氢聚变产生的能量少，" +
				             "但温度也更高，因此太阳的外层将膨胀，并且把一部分外层大气释放到太空中。当转向新元素的过程结束时，");
				textList.Add("太阳的质量将稍微下降，外层将延伸到地球或者火星目前运行的轨道处（这时由于太阳质量的下降，这两颗行星将会离太阳更远）。" +
				             "[url=http://baike.baidu.com/link?url=6veakMhSLq-nYndEhQc7ibn1aDnUzba0iVOaGFFLeTu6BruH6Jliqjki7Cd_CT2WazjmykjH-SWxVwnpzZhQM_][u]更多[/u][/url]");
			}
			if(menu.ID == 3){
				
				textList.Add("公转太阳绕银河系中心公转，绕银河系中心公转周期约2.5×10^8年。");
				textList.Add("银河系中心可能有巨大黑洞，但它周围布满了恒星，所以看上去象“银盘”。这些恒星都绕“银核”公转。与地球公转不同，这些恒星公转每绕一周离“银核”会更近。");
				textList.Add("太阳和其它天体一样，也在围绕自己的轴心自西向东自转，但观测和研究表明，太阳表面不同的纬度处，自转速度不一样。");
				textList.Add ("在赤道处，太阳自转一周需要25.4天，而在纬度40处需要27.2天，到了两极地区，自转一周则需要35天左右。这种自转方式被称为“较差自转”。");
				textList.Add("[url=http://baike.baidu.com/link?url=6veakMhSLq-nYndEhQc7ibn1aDnUzba0iVOaGFFLeTu6BruH6Jliqjki7Cd_CT2WazjmykjH-SWxVwnpzZhQM_][u]更多[/u][/url].");
			}
			
			if (menu.ID == 4) {
				
				textList.Add("太阳只是宇宙中一颗十分普通的恒星，但它却是太阳系的中心天体。太阳系中，包含我们的地球在内的八大行星、一些矮行星、彗星和其它无数的太阳系小天体，都在太阳的强大引力作用下环绕太阳运行。");
				textList.Add("太阳系的疆域庞大，仅以冥王星为例，其运行轨道距离太阳就将近40个天文单位，也就是60亿千米之遥远，而实际上太阳系的范围还要数十倍于此。但是这样一个庞大的太阳系家族，在银河系中却仅仅只是十分普通的沧海一粟。");
				textList.Add("	银河系拥有至少1000亿颗以上的恒星，直径约10万光年。太阳位于银道面之北的猎户座旋臂上，距离银河系中心约30000光年，在银道面以北约26光年，它一方面绕着银心以每秒250公里的速度旋转，周期大概是2.5亿年，另一方面又相对于周围恒星以每秒19.7公里的速度朝着织女星附近方向运动。 ");
				textList.Add("太阳也在自转，其周期在日面赤道带约25天；两极区约为35天。太阳正在穿越银河系内部边缘猎户臂的本地泡区中的本星际云。在距离地球17光年的距离内有50颗最邻近的恒星系（最接近的一颗是红矮星，被称为比邻星，距太阳大约4.2光年），太阳的质量在这些恒星中排在第四。");
				textList.Add("太阳在距离银河中心24000至26000光年的距离上绕着银河公转，从银河北极鸟瞰，太阳沿顺时针轨道运行，大约2亿2500万至2亿5000万年绕行一周。由于银河系在宇宙微波背景辐射（CMB）中以550公里/秒的速度朝向长蛇座的方向运动，这两个速度合成之后，太阳相对于CMB的速度是370公里/秒，朝向巨爵座或狮子座的方向运动。[url=http://baike.baidu.com/link?url=6veakMhSLq-nYndEhQc7ibn1aDnUzba0iVOaGFFLeTu6BruH6Jliqjki7Cd_CT2WazjmykjH-SWxVwnpzZhQM_][u]更多[/u][/url]");					
			}
		
		}

		//Earth
		if (Application.loadedLevel == 2) {
			if (menu.ID == 1) {
				textList.Add ("地球（英语：Earth）是太阳系八大行星之一（2006年冥王星被划为矮行星，因为其运动轨迹与其它八大行星不同），按离太阳由近及远的次序排为第三颗。它有一个天然卫星——月球，二者组成一个天体系统——地月系统。");
				textList.Add("地球作为一个行星，远在46亿年以前起源于原始太阳星云。地球会与外层空间的其他天体相互作用，包括太阳和月球。地球是上百万生物的家园，包括人类，地球是目前宇宙中已知存在生命的唯一天体。地球赤道半径6378.137千米，极半径6356.752千米，平均半径约6371千米，赤道周长大约为40076千米，地球上71%为海洋，29%为陆地，所以太空上看地球呈蓝色。地球是目前发现的星球中人类生存的唯一星球");
				textList.Add("[url=http://baike.baidu.com/link?url=6veakMhSLq-nYndEhQc7ibn1aDnUzba0iVOaGFFLeTu6BruH6Jliqjki7Cd_CT2WazjmykjH-SWxVwnpzZhQM_][u]更多[/u][/url]");
			}
			if(menu.ID == 3){
				
				textList.Add("地球存在绕自转轴自西向东的自转，平均角速度为每小时转动15度。在地球赤道上，自转的线速度是每秒465米。天空中各种天体东升西落的现象都是地球自转的反映。人们最早利用地球自转作为计量时间的基准。自20世纪以来由于天文观测技术的发展，人们发现地球自转是不均的。");
				textList.Add("1967年国际上开始建立比地球自转更为精确和稳定的原子时。由于原子时的建立和采用，地球自转中的各种变化相继被发现。天文学家已经知道地球自转速度存在长期减慢、不规则变化和周期性变化。");
				textList.Add("地球自转的周期性变化主要包括周年周期的变化，月周期、半月周期变化以及近周日和半周日周期的变化。周年周期变化，也称为季节性变化，是20世纪30年代发现的，它表现为春天地球自转变慢，秋天地球自转加快，其中还带有半年周期的变化。");
				textList.Add ("在赤道处，太阳自转一周需要25.4天，而在纬度40处需要27.2天，到了两极地区，自转一周则需要35天左右。这种自转方式被称为“较差自转”。");
				textList.Add ("地球公转的轨道是椭圆的，公转轨道半长径为149597870公里，轨道的偏心率为0.0167，公转的平均轨道速度为每秒29.79公里；公转的轨道面（黄道面）与地球赤道面的交角为23°27'，称为黄赤交角。地球自转产生了地球上的昼夜变化，地球公转及黄赤交角的存在造成了四季的交替。");
				textList.Add("[url=http://baike.baidu.com/link?url=6veakMhSLq-nYndEhQc7ibn1aDnUzba0iVOaGFFLeTu6BruH6Jliqjki7Cd_CT2WazjmykjH-SWxVwnpzZhQM_][u]更多[/u][/url].");
			}
			
			if (menu.ID == 4) {
				
				textList.Add("地球的未来与太阳有密切的关联，由于氦的灰烬在太阳的核心稳定的累积，太阳光度将缓慢的增加，在未来的11亿年中，太阳的光度将增加10%，之后的35亿年又将增加40%。");
				textList.Add("球表面温度的增加会加速无机的二氧化碳循环，使它的浓度在9亿年间还原至现存植物致死的水准（对C4光合作用是10 ppm）。而即使太阳是永恒和稳定的，地球内部持续的冷却，也会造成海洋和大气层的损失（由于火山活动降低）。");
				textList.Add("[url=http://baike.baidu.com/link?url=6veakMhSLq-nYndEhQc7ibn1aDnUzba0iVOaGFFLeTu6BruH6Jliqjki7Cd_CT2WazjmykjH-SWxVwnpzZhQM_][u]更多[/u][/url]");					
			}
			
		}

		
		//Moon
		if (Application.loadedLevel == 3) {
			if (menu.ID == 1) {
				textList.Add ("月球，俗称月亮，古称太阴，是环绕地球运行的一颗卫星。它是地球的一颗固态卫星，也是离地球最近的天体（与地球之间的平均距离是39万千米）。1969年尼尔·阿姆斯特朗和巴兹·奥尔德林成为最先登陆月球的人类。1969年9月美国“阿波罗11号”宇宙飞船返回地球.");
				textList.Add ("月球是被人们研究得最彻底的天体，至今第二个亲身到过的天体就是月球。月球的年龄大约已有46亿年。月球与地球一样有壳、幔、核等分层结构。");
				textList.Add ("最外层的月壳平均厚度约为60-65公里。月壳下面到1000公里深度是月幔，它占了月球的大部分体积。月幔下面是月核，月核的温度约为1000摄氏度，所以很可能是熔融状态的。月球直径约3474.8公里，大约是地球的1/4、太阳的1/400，月球到地球的距离相当于地球到太阳的距离的1/400，所以从地球上看月亮和太阳一样大。");
				textList.Add ("月球的体积大概有地球的1/49，质量约7350亿亿吨，差不多相当于地球质量的1/81，月球表面的重力约是地球重力的1/6。");
				textList.Add ("由于月球上没有大气，再加上月面物质的热容量和导热率又很低，因而月球表面昼夜的温差很大。白天，在阳光垂直照射的地方温度高达127℃；夜晚，温度可降低到-183℃。这些数值，只表示月球表面的温度。用射电观测可以测定月面土壤中的温度，这种测量表明，月面土壤中较深处的温度很少变化，这正是由于月面物质导热率低造成的。");
				textList.Add("[url=http://baike.baidu.com/link?url=6veakMhSLq-nYndEhQc7ibn1aDnUzba0iVOaGFFLeTu6BruH6Jliqjki7Cd_CT2WazjmykjH-SWxVwnpzZhQM_][u]更多[/u][/url]");
			}
			if(menu.ID == 3){
				textList.Add("月球是距离地球最近的天体，它与地球的平均距离约为384401千米。它的平均直径约为3476千米，地球直径的3/11。");
				textList.Add("月球的表面积有3800万Km²，还不如亚洲的面积大。月球的质量约7350亿亿吨，相当于地球质量的1/81，月面重力则差不多相当于地球重力的1/6。");
				textList.Add("月面的直径大约是地球的1/4.月球的体积大约是地球的1/49.然而，月球以每年3.8厘米的速度，远离地球。这就意味着，总有一天月球会离开我们，但需要几十亿年。");
				textList.Add("月球以椭圆轨道绕地球运转。这个轨道平面在天球上截得的大圆称“白道”。白道平面不重合于天赤道，也不平行于黄道面，而且空间位置不断变化。周期173日。月球轨道（白道）对地球轨道（黄道）的平均倾角为5°09′。");
				textList.Add("月球在绕地球公转的同时进行自转，周期27.32166日，正好是一个恒星月，所以我们看不见月球背面。这种现象我们称“同步自转”，几乎是卫星世界的普遍规律。一般认为是行星对卫星长期潮汐作用的结果。天平动是一个很奇妙的现象，它使得我们得以看到59%的月面。");
				textList.Add("[url=http://baike.baidu.com/link?url=6veakMhSLq-nYndEhQc7ibn1aDnUzba0iVOaGFFLeTu6BruH6Jliqjki7Cd_CT2WazjmykjH-SWxVwnpzZhQM_][u]更多[/u][/url].");
			}
			
			if (menu.ID == 4) {

				textList.Add("2014年12月09日，最新研究发现，月球的内核在30多亿年前是一个强大的“发电机”，产生了强烈的磁场，远高于地球的磁场。");
				textList.Add("最新证据显示，大约在35.6至42亿年前，月球的内核是一种熔融状态，创造了持续近10亿年的磁场，而比现在地球的磁场强烈。此前在阿波罗登月时，宇航员从月球表面带回了岩石样本。天文学家对样本进行分析后推测，月球核心应该存在一个能生成磁场的超级“发电机”。");
				textList.Add("但是，科学家依旧不能解释如此强大的月球磁场是如何逐渐消失的。如果能解开这一秘密，或许能预测地球磁场的未来发展。");
				textList.Add("2015年4月17日，中国科学家通过“玉兔”月球车的探测范围在月球表面雨海盆地，研究了稀土和放射性元素的分布，填补了美俄对月球地质研究的空白。");
				textList.Add("[url=http://baike.baidu.com/link?url=6veakMhSLq-nYndEhQc7ibn1aDnUzba0iVOaGFFLeTu6BruH6Jliqjki7Cd_CT2WazjmykjH-SWxVwnpzZhQM_][u]更多[/u][/url]");					
			}
			
		}



		m_bInit = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!m_bInit)
			return;
	}
}
