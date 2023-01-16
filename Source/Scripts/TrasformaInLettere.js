function digit3(num)
{
	var n0_19=new Array("","uno","due","tre","quattro","cinque","sei","sette","otto","nove","dieci","undici","dodici","tredici","quattordici","quindici","sedici","diciassette","diciotto","diciannove");
	var n10a=new Array("","","venti","trenta","quaranta","cinquanta","sessanta","settanta","ottanta","novanta","cento"),n10b=new Array("","","vent","trent","quarant","cinquant","sessant","settant","ottant","novant","cento");
	var str="";
	if(num.charAt(0)!='0')
		str+=((num.charAt(0)!='1')?n0_19[num.charAt(0)]:"")+((num.charAt(1)=='8')?n10b[10]:n10a[10]);
	if(num.charAt(1)>'1')
		str+=((num.charAt(2)=='1' || num.charAt(2)=='8')?n10b[num.charAt(1)]:n10a[num.charAt(1)])+n0_19[num.charAt(2)];
	else
		str+=n0_19[parseInt(num.substring(1,3),10)];
	return str;
}


function TrasformaInLettere(num,lower)
{
	var pto1=new Array("","uno","mille","unmilione","unmiliardo","mille","unmilione"),pto2=new Array("","","mila","milioni","miliardi","mila","milioni");
	var pto3=new Array("","undecimo","uncentesimo","unmillesimo"),pto4=new Array("","decimi","centesimi","millesimi");
	if(isNaN(num))
		return null;
	num=String(num);
	for(var i=0;num.charAt(i)=='0';++i)
		;
	var pt=num.lastIndexOf("."),num1=num.substring((i==pt)?(i-1):i,(pt<0)?num.length:pt);
	num1="000".substring((num1.length%3)?num1.length%3:3)+num1;
	if(!parseInt(num1,10) && pt<0)
		return (lower)?"zero":"Zero";
	var str1="",str2="",j=num1.length/3;
	if(j>6)
		return null;
	for(i=0;j;--j,i+=3){
		if(parseInt(num1.substring(i,i+3),10)==1)
			str1+=pto1[j];
		else if(!parseInt(num1.substring(i,i+3),10) && j==4)
			str1+=((!parseInt(num1.substring(i-3,i+3),10))?"di":"")+pto2[j];
		else{
			str1+=digit3(num1.substring(i,i+3));
			if(parseInt(num1.substring(i,i+3),10))
				str1+=pto2[j];
		}
	}
	if(pt>-1){
		var num2=num.substring(pt+1,pt+4);
		if(parseInt(num2,10)>0)
			str2=((str1)?" e ":"")+((parseInt(num2,10)>1)?(digit3("000".substring((num2.length%3)?num2.length%3:3)+num2)+pto4[num2.length]):pto3[num2.length]);
		else if(!str1)
			str2="zero";
		if(!str1)
			str2=(lower)?str2:(str2.charAt(0).toUpperCase()+str2.substring(1));
	}
	return ((lower)?str1:(str1.charAt(0).toUpperCase()+str1.substring(1)))+str2;
}