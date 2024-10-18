using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Numerics;

public class SpaceDriveCalc
{
    //** 連続計算時の前回位置を覚えている
    DriveAxisPos FPrevAxis;
    double OffsetA;
    bool IsRetract;
    double RetractC; //C軸のリトラクト判定値

    public class DriveAxisPos
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double A { get; set; }
        public double C { get; set; }

        public bool CAny = false;
        public bool AAny = false;

        // コンストラクタ
        public DriveAxisPos(double x, double y, double z, double a, double c, bool aany, bool cany)
        {
            X = x;
            Y = y;
            Z = z;
            A = a;
            C = c;
            AAny = aany;
            CAny = cany;
        }

        // ToString() メソッド：オブジェクトの内容を文字列として返す
        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z: {Z}, A: {A}, C: {C}, AAny: {AAny}, CAny: {CAny}";
        }

        // IsEqual() メソッド：2つのDriveAxisPosオブジェクトが同じかどうかを判定する
        public bool IsEqual(DriveAxisPos other)
        {
            if (other == null)
                return false;

            return (X == other.X && Y == other.Y && Z == other.Z && A == other.A && C== other.C);
            //4軸が同じか
            /*
            bool cmp = (X == other.X && Y == other.Y && Z == other.Z && A == other.A);
            if (cmp)
            {
                if (CAny == true)
                {
                    //CがAnyの場合
                    return (other.CAny == true);
                }
                else                
                {
                    //CがAnyでない場合
                    return (other.CAny ==false && C==other.C);
                }
            }
            return false;
            */
        }
    }

    static public double DegToRad( double aDeg)
    {
        return aDeg * Math.PI / 180;
    }

    static public double RadToDeg(double aRad)
    {
        return aRad * (180 / Math.PI);
    }


    public void Calc(Vector3 aPos, Vector3 aNor, DriveAxisPos aDriveAxis, bool aFirst)
    {
        // ここで、aPos（位置）、aNor（法線）、aDriveAxis（駆動軸の位置）に基づいて計算を実行します
        // 仮の計算
        //Vector3 p = Vector3.Normalize(aPos);
        //Vector3 v = Vector3.Normalize(aNor);

        double rad_A, sin_A, cos_A, rad_C, sin_C, cos_C;
        double cb1, cb2;

        /*
        begin
          result:= false;
        if (a_bFirst) then
            FPrevAxis.init(true);   // 全軸0で初期化

        //方向ベクトルが０ベクトルならZ方向に向ける
        if (aVector.x = 0) and(aVector.y = 0) and(aVector.z = 0) then begin
          aVector.z:= 1;
        end;
        */

        //C軸 ---------------------------------------------------------------------
        if (aNor.X == 0 && aNor.Y == 0)
        {
            aDriveAxis.CAny = true;

            if (aFirst)
                aDriveAxis.C = 0;
            else
                aDriveAxis.C = FPrevAxis.C;

            rad_C = DegToRad(aDriveAxis.C);
        }
        else
        {
            rad_C = Math.Atan2(aNor.X, aNor.X); // TODO Delphiと.netの仕様をチェック
            aDriveAxis.C = RadToDeg(rad_C);
        }

        sin_C= Math.Sin(rad_C);
        cos_C= Math.Cos(rad_C);

        //A軸 ---------------------------------------------------------------------
        cb1 = sin_C * aNor.X + cos_C * aNor.Y;

        if (cb1 == 0 && aNor.Z == 0) {
            aDriveAxis.AAny= true;                         //A軸は任意
            if (aFirst)
                aDriveAxis.A = 0;
            else
                aDriveAxis.A = FPrevAxis.A;
            rad_A = DegToRad(aDriveAxis.A);
        }
        else
        {
            rad_A = Math.Atan2(cb1, aNor.Z); // TODO Delphiと.netの仕様をチェック
            aDriveAxis.A= RadToDeg(rad_A);
        }

        sin_A= Math.Sin(rad_A);
        cos_A= Math.Cos(rad_A);

        //X,Y,Z軸 -----------------------------------------------------------------
        cb1= sin_C * aPos.X + cos_C * aPos.Y;
        cb2= aPos.Z + OffsetA;
        aDriveAxis.X = cos_C * aPos.X - sin_C * aPos.Y;
        aDriveAxis.Y = cos_A * cb1 - sin_A * cb2;
        aDriveAxis.Z = sin_A * cb1 + cos_A * cb2 - OffsetA;

        //前回の値と比較して、+-180度境界をまたぐかどうか TODO 180度ではないのでは？
        if (!aFirst )
        {
            while (true)
            {
                if (Math.Abs(aDriveAxis.C - FPrevAxis.C) > 330)
                {
                    if (aDriveAxis.C > FPrevAxis.C)
                        aDriveAxis.C= aDriveAxis.C - 360;
                    else
                        aDriveAxis.C= aDriveAxis.C + 360;
                }
                else break;
            }

            //前回の値と比較して、RetractC以上なら、リトラクトフラグを立てる
            IsRetract= Math.Abs(aDriveAxis.C - FPrevAxis.C) > RetractC;
        }
        else
        {
            IsRetract = false;
        }

        Console.WriteLine($"Pos: {aPos}");
        Console.WriteLine($"Nor: {aNor}");
        Console.WriteLine($"DriveAxis: {aDriveAxis}");
    }
}
