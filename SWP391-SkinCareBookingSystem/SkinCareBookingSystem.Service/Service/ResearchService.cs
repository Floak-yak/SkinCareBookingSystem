using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Service
{
    public class SkincareSurvey
    {
        private Node root;

        public SkincareSurvey()
        {
            BuildTree();
        }

        private void BuildTree()
        {
            root = new Node("Bạn có biết loại da của mình không?");

            var knowSkinType = new Node("Chọn loại da của bạn: Da dầu, Da khô, Da hỗn hợp, Da nhạy cảm, Da thường");
            var notSure = new Node("Da bạn thường cảm thấy như thế nào vào buổi sáng?");

            root.Next["Có"] = knowSkinType;
            root.Next["Không"] = notSure;

            // Nếu không biết, xác định loại da
            notSure.Next["Khô, căng"] = new Node("Da khô") { Recommendation = "Dưỡng ẩm sâu, dùng sữa rửa mặt dịu nhẹ." };
            notSure.Next["Nhờn, bóng dầu"] = new Node("Da dầu") { Recommendation = "Kiểm soát dầu, dùng BHA, sữa rửa mặt kiềm dầu." };
            notSure.Next["Vùng chữ T dầu nhưng má khô"] = new Node("Da hỗn hợp") { Recommendation = "Dưỡng ẩm vừa phải, kiểm soát dầu vùng chữ T." };
            notSure.Next["Không cảm giác đặc biệt"] = new Node("Da thường") { Recommendation = "Duy trì dưỡng da cân bằng." };

            // Nếu biết loại da -> Tiếp tục hỏi vấn đề da
            var issueQuestion = new Node("Bạn có gặp vấn đề nào sau đây? (Mụn, Lỗ chân lông to, Nhạy cảm, Lão hóa, Không có vấn đề)");
            knowSkinType.Next["Da dầu"] = issueQuestion;
            knowSkinType.Next["Da khô"] = issueQuestion;
            knowSkinType.Next["Da hỗn hợp"] = issueQuestion;
            knowSkinType.Next["Da nhạy cảm"] = issueQuestion;
            knowSkinType.Next["Da thường"] = issueQuestion;

            // Gợi ý theo vấn đề da
            issueQuestion.Next["Mụn"] = new Node("Chăm sóc da mụn") { Recommendation = "Dùng BHA, Niacinamide, tránh dầu gây bít tắc." };
            issueQuestion.Next["Lỗ chân lông to"] = new Node("Giảm lỗ chân lông") { Recommendation = "Dùng Niacinamide, BHA, dưỡng ẩm nhẹ." };
            issueQuestion.Next["Nhạy cảm"] = new Node("Da nhạy cảm") { Recommendation = "Dùng Ceramide, tránh cồn và hương liệu." };
            issueQuestion.Next["Lão hóa"] = new Node("Chống lão hóa") { Recommendation = "Dùng Retinol, Vitamin C, chống nắng kỹ." };
            issueQuestion.Next["Không có vấn đề"] = new Node("Duy trì routine") { Recommendation = "Duy trì skincare cơ bản: Sữa rửa mặt, dưỡng ẩm, chống nắng." };
        }

        public void StartSurvey()
        {
            Node current = root;
            while (current != null)
            {
                Console.WriteLine(current.Question);
                if (current.Recommendation != null)
                {
                    Console.WriteLine("\nGợi ý skincare: " + current.Recommendation);
                    break;
                }
                string answer = Console.ReadLine();
                if (current.Next.ContainsKey(answer))
                {
                    current = current.Next[answer];
                }
                else
                {
                    Console.WriteLine("Lựa chọn không hợp lệ, vui lòng nhập lại.");
                }
            }
        }
    }
}
