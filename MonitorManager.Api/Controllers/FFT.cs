using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitorManager.Api.Controllers
{
    public class FFT
    {
        // 快速傅里叶变换
        // 作者：51
        // ComplexList src：数据输入
        // ref int lenght：欲变换数据的长度，函数调用后此值更改为实际变换长度
        // int flag：区分fft或dtft，为1为fft，为-1为idft
        public ComplexList fft_core(ComplexList src, ref int lenght, int flag)
        {
            //按时间抽取FFT方法(DIT)

            //补零后长度
            int relog2N = ReLog2N(lenght);

            int bitlenghth = relog2N;
            int N = 0x01 << bitlenghth;

            //重新复制数据，同时进行
            //    逆序排放，并补零
            int index;
            ComplexList Register = new ComplexList(N);
            for (int i = 0; i < N; i++)
            {
                index = ReArrange(i, bitlenghth);
                Register[i] = (index < src.Lenght) ? src[index] : new Complex(0);
            }

            //生成WN表，以免运行时进行重复计算
            ComplexList WN = new ComplexList(N / 2);
            for (int i = 0; i < N / 2; i++)
            {
                WN[i] = new Complex(Math.Cos(2 * Math.PI / N * i), -flag * Math.Sin(2 * Math.PI / N * i));
            }

            //蝶形运算
            int Index0, Index1;
            Complex temp;
            for (int steplenght = 2; steplenght <= N; steplenght *= 2)
            {
                for (int step = 0; step < N / steplenght; step++)
                {
                    for (int i = 0; i < steplenght / 2; i++)
                    {
                        Index0 = steplenght * step + i;
                        Index1 = steplenght * step + i + steplenght / 2;

                        temp = Register[Index1] * WN[N / steplenght * i];
                        Register[Index1] = Register[Index0] - temp;
                        Register[Index0] = Register[Index0] + temp;
                    }
                }
            }

            //若为idft
            if (-1 == flag)
            {
                for (int i = 0; i < Register.Lenght; i++)
                {
                    Register[i] /= N;
                }
            }

            //赋值
            lenght = N;

            /*
            //清理内存
            WN = null;
            temp = null;
            GC.Collect();
            */

            //返回
            return Register;
        }


        // 获取按位逆序，bitlenght为数据长度
        // fft函数内使用
        private static int ReArrange(int dat, int bitlenght)
        {
            int ret = 0;
            for (int i = 0; i < bitlenght; i++)
            {
                if (0 != (dat & (0x01 << i))) ret |= ((0x01 << (bitlenght - 1)) >> i);
            }
            return ret;
        }

        // 获取扩展长度后的幂次
        // 由于fft要求长度为2^n，所以用此函数来获取所需长度
        public static int ReLog2N(int count)
        {
            int log2N = 0;
            uint mask = 0x80000000;
            for (int i = 0; i < 32; i++)
            {
                if (0 != ((mask >> i) & count))
                {
                    if ((mask >> i) == count) log2N = 31 - i;
                    else log2N = 31 - i + 1;
                    break;
                }
            }
            return log2N;
        }






        /// <summary>
        /// 一维频率抽取基2快速傅里叶变换
        /// 频率抽取：输入为自然顺序，输出为码位倒置顺序
        /// 基2：待变换的序列长度必须为2的整数次幂
        /// </summary>
        /// <param name="sourceData">待变换的序列(复数数组)</param>
        /// <param name="countN">序列长度,可以指定[0,sourceData.Length-1]区间内的任意数值</param>
        /// <returns>返回变换后的序列（复数数组）</returns>
        public Complex[] fft_frequency(Complex[] sourceData, int countN)
        {
            //2的r次幂为N，求出r.r能代表fft算法的迭代次数
            int r = Convert.ToInt32(Math.Log(countN, 2));


            //分别存储蝶形运算过程中左右两列的结果
            Complex[] interVar1 = new Complex[countN];
            Complex[] interVar2 = new Complex[countN];
            interVar1 = (Complex[])sourceData.Clone();

            //w代表旋转因子
            Complex[] w = new Complex[countN / 2];
            //为旋转因子赋值。（在蝶形运算中使用的旋转因子是已经确定的，提前求出以便调用）
            //旋转因子公式 \  /\  /k __
            //              \/  \/N  --  exp(-j*2πk/N)
            //这里还用到了欧拉公式
            for (int i = 0; i < countN / 2; i++)
            {
                double angle = -i * Math.PI * 2 / countN;
                w[i] = new Complex(Math.Cos(angle), Math.Sin(angle));
            }

            //蝶形运算
            for (int i = 0; i < r; i++)
            {
                //i代表当前的迭代次数，r代表总共的迭代次数.
                //i记录着迭代的重要信息.通过i可以算出当前迭代共有几个分组，每个分组的长度

                //interval记录当前有几个组
                // <<是左移操作符，左移一位相当于*2
                //多使用位运算符可以人为提高算法速率^_^
                int interval = 1 << i;

                //halfN记录当前循环每个组的长度N
                int halfN = 1 << (r - i);

                //循环，依次对每个组进行蝶形运算
                for (int j = 0; j < interval; j++)
                {
                    //j代表第j个组

                    //gap=j*每组长度，代表着当前第j组的首元素的下标索引
                    int gap = j * halfN;

                    //进行蝶形运算
                    for (int k = 0; k < halfN / 2; k++)
                    {
                        interVar2[k + gap] = interVar1[k + gap] + interVar1[k + gap + halfN / 2];
                        interVar2[k + halfN / 2 + gap] = (interVar1[k + gap] - interVar1[k + gap + halfN / 2]) * w[k * interval];
                    }
                }

                //将结果拷贝到输入端，为下次迭代做好准备
                interVar1 = (Complex[])interVar2.Clone();
            }

            //将输出码位倒置
            for (uint j = 0; j < countN; j++)
            {
                //j代表自然顺序的数组元素的下标索引

                //用rev记录j码位倒置后的结果
                uint rev = 0;
                //num作为中间变量
                uint num = j;

                //码位倒置（通过将j的最右端一位最先放入rev右端，然后左移，然后将j的次右端一位放入rev右端，然后左移...）
                //由于2的r次幂=N，所以任何j可由r位二进制数组表示，循环r次即可
                for (int i = 0; i < r; i++)
                {
                    rev <<= 1;
                    rev |= num & 1;
                    num >>= 1;
                }
                interVar2[rev] = interVar1[j];
            }
            return interVar2;

        }
    }
    public class ComplexList
    {
        double[] _ComplexList_Re;
        double[] _ComplexList_Im;
        public int Lenght { get; private set; }
        public Complex this[int Index]
        {
            get
            {
                return new Complex(_ComplexList_Re[Index], _ComplexList_Im[Index]);
            }
            set
            {
                _ComplexList_Re[Index] = ((Complex)value).Re;
                _ComplexList_Im[Index] = ((Complex)value).Im;
            }
        }

        public ComplexList(int lenght)
        {
            Lenght = lenght;
            _ComplexList_Re = new double[Lenght];
            _ComplexList_Im = new double[Lenght];
        }
        public ComplexList(double[] re)
        {
            Lenght = re.Length;
            _ComplexList_Re = re;
            _ComplexList_Im = new double[Lenght];
        }
        public ComplexList(double[] re, double[] im)
        {
            Lenght = Math.Max(re.Length, im.Length);
            if (re.Length == im.Length)
            {
                _ComplexList_Re = re;
                _ComplexList_Im = im;
            }
            else
            {
                _ComplexList_Re = new double[Lenght];
                _ComplexList_Im = new double[Lenght];
                for (int i = 0; i < re.Length; i++) _ComplexList_Re[i] = re[i];
                for (int i = 0; i < im.Length; i++) _ComplexList_Im[i] = im[i];
            }
        }

        public void Clear()
        {
            Array.Clear(_ComplexList_Re, 0, _ComplexList_Re.Length);
            Array.Clear(_ComplexList_Im, 0, _ComplexList_Im.Length);
        }

        public double[] GetRePtr()
        {
            return _ComplexList_Re;
        }
        public double[] GetImPtr()
        {
            return _ComplexList_Im;
        }
        public ComplexList Clone()
        {
            return new ComplexList((double[])(_ComplexList_Re.Clone()), (double[])(_ComplexList_Im.Clone()));
        }
        public ComplexList GetAmplitude()
        {
            double[] amp = new double[Lenght];
            for (int i = 0; i < Lenght; i++)
            {
                amp[i] = this[i].Modulus();
            }
            return new ComplexList(amp);
        }
    }
    public class Complex
    {
        public double Re;
        public double Im;
        public Complex()
        {
            Re = 0;
            Im = 0;
        }
        public Complex(double re)
        {
            Re = re;
            Im = 0;
        }
        public Complex(double re, double im)
        {
            Re = re;
            Im = im;
        }

        public double Modulus()
        {
            return Math.Sqrt(Re * Re + Im * Im);
        }

        public override string ToString()
        {
            string retStr;
            if (Math.Abs(Im) < 0.0001) retStr = Re.ToString("f4");
            else if (Math.Abs(Re) < 0.0001)
            {
                if (Im > 0) retStr = "j" + Im.ToString("f4");
                else retStr = "- j" + (0 - Im).ToString("f4");
            }
            else
            {
                if (Im > 0) retStr = Re.ToString("f4") + "+ j" + Im.ToString("f4");
                else retStr = Re.ToString("f4") + "- j" + (0 - Im).ToString("f4");
            }
            retStr += " ";
            return retStr;
        }

        //操作符重载
        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.Re + c2.Re, c1.Im + c2.Im);
        }
        public static Complex operator +(double d, Complex c)
        {
            return new Complex(d + c.Re, c.Im);
        }
        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.Re - c2.Re, c1.Im - c2.Im);
        }
        public static Complex operator -(double d, Complex c)
        {
            return new Complex(d - c.Re, -c.Im);
        }
        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex(c1.Re * c2.Re - c1.Im * c2.Im, c1.Re * c2.Im + c2.Re * c1.Im);
        }
        public static Complex operator *(Complex c, double d)
        {
            return new Complex(c.Re * d, c.Im * d);
        }
        public static Complex operator *(double d, Complex c)
        {
            return new Complex(c.Re * d, c.Im * d);
        }
        public static Complex operator /(Complex c, double d)
        {
            return new Complex(c.Re / d, c.Im / d);
        }
        public static Complex operator /(double d, Complex c)
        {
            double temp = d / (c.Re * c.Re + c.Im * c.Im);
            return new Complex(c.Re * temp, -c.Im * temp);
        }
        public static Complex operator /(Complex c1, Complex c2)
        {
            double temp = 1 / (c2.Re * c2.Re + c2.Im * c2.Im);
            return new Complex((c1.Re * c2.Re + c1.Im * c2.Im) * temp, (-c1.Re * c2.Im + c2.Re * c1.Im) * temp);
        }
    }
}