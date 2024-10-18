using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
//using MathNet.Spatial;
//using MathNet.Spatial.Euclidean;
//using MathNet.Spatial.Units;

//問題点　Quaternionが float(doubleの半分)で実装されている。

namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 元のベクトル v(vx, vy, vz)
            Vector3 v = new Vector3(1, 0, 0); // 例: X軸に沿ったベクトル

            // 変換先のベクトル p(px, py, pz)
            Vector3 p = new Vector3(0, 0, 1); // 例: Y軸に沿ったベクトル

            // クオータニオンを求めるための手順
            Quaternion q = CalculateQuaternion(v, p);

            // 求めたクオータニオンからYaw, Pitch, Rollを計算
            var (yaw, pitch, roll) = QuaternionToEulerAngles(q);
            //var (yaw, pitch, roll) = QuaternionToYawRoll(q);

            // 結果を表示
            //var t360 = 360 / (2 * Math.PI);

            Console.WriteLine("Quaternion: " + q);
            Console.WriteLine("Yaw (Z axis rotation): " + yaw + " degrees");
            Console.WriteLine("Pitch (Y axis rotation): " + pitch + " degrees");
            Console.WriteLine("Roll (X axis rotation): " + roll + " degrees");
            Console.ReadKey();
        }


        // v から p への回転を表すクオータニオンを計算
        static Quaternion CalculateQuaternion(Vector3 v, Vector3 p)
        {
            // 正規化（クオータニオン計算には単位ベクトルが必要）
            v = Vector3.Normalize(v);
            p = Vector3.Normalize(p);

            // 2つのベクトル間の回転軸を計算
            Vector3 axis = Vector3.Cross(v, p);
            double angle = Math.Acos( Vector3.Dot(v, p));

            // クオータニオンを回転軸と角度から作成
            Quaternion q = Quaternion.CreateFromAxisAngle(axis, (float) angle);
            return q;
        }

        // クオータニオンからYaw, Pitch, Rollを計算
        static (double yaw, double pitch, double roll) QuaternionToEulerAngles(Quaternion q)
        {
            // クオータニオンの要素を取得
            double qx = q.X;
            double qy = q.Y;
            double qz = q.Z;
            double qw = q.W;

            // Yaw (Z軸の回転)
            double yaw = Math.Atan2(2.0 * (qw * qz + qx * qy), 1.0 - 2.0 * (qy * qy + qz * qz));

            // Pitch (Y軸の回転)
            double pitch = Math.Asin(2.0 * (qw * qy - qz * qx));

            // Roll (X軸の回転)
            double roll = Math.Atan2(2.0 * (qw * qx + qy * qz), 1.0 - 2.0 * (qx * qx + qy * qy));

            // ラジアンから度に変換
            yaw = yaw * (180.0 / Math.PI);
            pitch = pitch * (180.0 / Math.PI);
            roll = roll * (180.0 / Math.PI);

            return (yaw, pitch, roll);
        }

        // クオータニオンからYaw, Rollを計算（Pitchを常に0に設定）＝思った結果にならない
        static (double yaw, double pitch, double roll) QuaternionToYawRoll(Quaternion q)
        {
            // クオータニオンの要素を取得
            double qx = q.X;
            double qy = q.Y;
            double qz = q.Z;
            double qw = q.W;

            // Yaw (Z軸の回転)
            double yaw = Math.Atan2(2.0 * (qw * qz + qx * qy), 1.0 - 2.0 * (qy * qy + qz * qz));

            // Roll (X軸の回転)
            double roll = Math.Atan2(2.0 * (qw * qx + qy * qz), 1.0 - 2.0 * (qx * qx + qy * qy));

            // Pitchは0に固定
            double pitch = 0;

            // ラジアンから度に変換
            yaw = yaw * (180.0 / Math.PI);
            roll = roll * (180.0 / Math.PI);


            return (yaw, pitch, roll);
        }


    }


}


