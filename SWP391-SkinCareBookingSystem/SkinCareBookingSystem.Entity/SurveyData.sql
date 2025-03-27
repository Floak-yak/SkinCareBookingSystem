-- SQL Script to seed the skin test data into the database
-- This script inserts data into Surveys, Options, and Nodes tables

-- Clear existing data (if needed)
DELETE FROM [Options] WHERE 1=1;
DELETE FROM [Nodes] WHERE 1=1;
DELETE FROM [Surveys] WHERE 1=1;

-- Set IDENTITY_INSERT to ON for Surveys table
SET IDENTITY_INSERT [Surveys] ON;

-- Insert Survey questions
INSERT INTO [Surveys] (QuestionId, QuestionIdentifier, Question, IsResult) VALUES
-- Main questions
(1, 'Q1', 'Sau khi rửa mặt khoảng 30 phút, bạn thấy tình trạng da như thế nào?', 0),

-- Da Khô branch
(2, 'Q2A', 'Bạn có thường xuyên thấy da bong tróc ở khóe môi, hai bên má hoặc vùng mũi không?', 0),
(3, 'Q3A', 'Da bạn có hay ngứa rát, châm chích khi bôi mỹ phẩm mới hoặc khi trời trở lạnh?', 0),

-- Da Dầu branch
(4, 'Q2B', 'Sau khi rửa mặt khoảng 1 tiếng, bạn có cảm thấy da lại bóng nhẫy không?', 0),
(5, 'Q3B', 'Bạn có dễ bị mẩn đỏ, ngứa khi dùng sản phẩm chứa cồn hay hương liệu mạnh không?', 0),

-- Da Hỗn Hợp branch
(6, 'Q2C', 'Bạn cảm thấy vùng má của mình thế nào (so với vùng chữ T)?', 0),
(7, 'Q3C', 'Da bạn có hay nổi mẩn đỏ ở vùng má hoặc hai bên cánh mũi khi thời tiết thay đổi?', 0),

-- Da Thường branch
(8, 'Q2D', 'Bạn có hay gặp vấn đề về da như nổi mụn, mẩn đỏ hoặc bong tróc không?', 0),
(9, 'Q3D', 'Khi ở môi trường điều hòa/lạnh cả ngày, da bạn phản ứng thế nào?', 0),

-- Da Nhạy Cảm branch
(10, 'Q2E', 'Bạn từng bị nổi mẩn, ngứa, hoặc châm chích mạnh khi thử sản phẩm mới chưa?', 0),
(11, 'Q3E', 'Ngoài độ nhạy cảm, da bạn thiên về khô hay dầu hơn?', 0),

-- Result nodes
(12, 'RESULT_DA_KHO', 'KẾT LUẬN: DA BẠN THIÊN VỀ KHÔ.\n\n- Dễ mất nước, bong tróc nếu không dưỡng ẩm.\n- Nên dùng kem dưỡng giàu ẩm, uống đủ nước.\n- Ưu tiên sản phẩm dịu nhẹ, tránh tẩy rửa mạnh.\n\nTips: Thoa kem dưỡng khi da còn ẩm để ''khóa'' độ ẩm tốt hơn.', 1),
(13, 'RESULT_DA_KHO_NHAY_CAM', 'KẾT LUẬN: DA BẠN KHÔ & NHẠY CẢM.\n\n- Da khô thiếu ẩm + dễ kích ứng.\n- Tránh sản phẩm chứa hương liệu, cồn, AHA/BHA quá mạnh.\n- Nên test sản phẩm ở vùng nhỏ trước khi dùng toàn mặt.\n\nTips: Tăng cường hàng rào bảo vệ da với kem dưỡng phục hồi.', 1),
(14, 'RESULT_DA_DAU', 'KẾT LUẬN: DA BẠN THIÊN VỀ DẦU.\n\n- Dễ đổ dầu, lỗ chân lông to.\n- Chú ý làm sạch sâu, nhưng đừng quên cấp ẩm.\n- Ưu điểm: Da dầu thường chậm lão hóa hơn.\n\nTips: Dùng toner cân bằng, sản phẩm kiềm dầu, mask đất sét định kỳ.', 1),
(15, 'RESULT_DA_DAU_NHAY_CAM', 'KẾT LUẬN: DA BẠN DẦU & NHẠY CẢM.\n\n- Vừa đổ dầu, vừa dễ kích ứng.\n- Tránh tẩy rửa quá mạnh, chú trọng duy trì pH cân bằng.\n- Sản phẩm kiềm dầu nhẹ nhàng, tránh cồn & hương liệu.\n\nTips: Chọn gel dưỡng mỏng nhẹ để không bít tắc, nhưng vẫn có hàng rào bảo vệ da.', 1),
(16, 'RESULT_DA_HON_HOP', 'KẾT LUẬN: DA BẠN THUỘC HỖN HỢP.\n\n- Vùng chữ T đổ dầu, vùng má khô hoặc bình thường.\n- Cần chăm sóc ''phân vùng'': kiềm dầu vùng T, cấp ẩm vùng má.\n\nTips: Có thể dùng 2 loại mặt nạ riêng cho T-zone và má (multimasking).', 1),
(17, 'RESULT_DA_HON_HOP_NHAY_CAM', 'KẾT LUẬN: DA BẠN HỖN HỢP & NHẠY CẢM.\n\n- Vừa phải cân bằng dầu - khô, vừa chú ý thành phần dịu nhẹ.\n- Dễ mẩn đỏ ở vùng má hoặc cánh mũi.\n\nTips: Tránh layering quá nhiều sản phẩm cùng lúc, test kỹ khi đổi mỹ phẩm.', 1),
(18, 'RESULT_DA_THUONG', 'KẾT LUẬN: DA BẠN THUỘC DA THƯỜNG.\n\n- Tương đối cân bằng, ít khô căng hoặc đổ dầu.\n- Đừng chủ quan, vẫn cần chăm sóc cơ bản: làm sạch - dưỡng ẩm - chống nắng.\n\nTips: Bạn có lợi thế lớn, chỉ cần duy trì thói quen skincare hợp lý là xịn rồi!', 1),
(19, 'RESULT_DA_THUONG_NHAY_CAM', 'KẾT LUẬN: DA BẠN THƯỜNG & NHẠY CẢM.\n\n- Da không quá dầu/khô nhưng vẫn dễ kích ứng bởi tác nhân bên ngoài.\n- Chú ý chọn mỹ phẩm dịu nhẹ, không chứa nhiều hương liệu.\n\nTips: Test sản phẩm mới ở vùng quai hàm/cổ tay trước khi bôi khắp mặt.', 1);

-- Turn IDENTITY_INSERT OFF for Surveys table
SET IDENTITY_INSERT [Surveys] OFF;

-- Insert options for each question
INSERT INTO [Options] (Id, Label, NextQuestionId, QuestionId) VALUES
-- Options for Q1
('Q1_A', 'A. Bị khô căng, thậm chí bong tróc', 'Q2A', 1),
('Q1_B', 'B. Bóng dầu nhiều, đặc biệt ở vùng chữ T', 'Q2B', 1),
('Q1_C', 'C. Vùng chữ T có dầu nhưng vùng má lại khô (hoặc bình thường)', 'Q2C', 1),
('Q1_D', 'D. Tương đối cân bằng, không quá khô, cũng không quá dầu', 'Q2D', 1),
('Q1_E', 'E. Thường hay bị ửng đỏ, mẩn hoặc châm chích khi thay đổi sản phẩm/môi trường', 'Q2E', 1),

-- Options for Q2A (Da Khô)
('Q2A_1', 'Rất thường xuyên, da bong tróc rõ rệt', 'Q3A', 2),
('Q2A_2', 'Thỉnh thoảng thôi, không quá liên tục', 'Q3A', 2),

-- Options for Q3A
('Q3A_1', 'Có, mình dễ kích ứng, mẩn đỏ hoặc châm chích', 'RESULT_DA_KHO_NHAY_CAM', 3),
('Q3A_2', 'Không, chỉ khô nhưng ít khi bị kích ứng', 'RESULT_DA_KHO', 3),

-- Options for Q2B (Da Dầu)
('Q2B_1', 'Có, hầu như lúc nào cũng bóng dầu trở lại', 'Q3B', 4),
('Q2B_2', 'Bớt dầu hơn chút, nhưng vùng chữ T vẫn rõ dầu', 'Q3B', 4),

-- Options for Q3B
('Q3B_1', 'Có, mình dễ bị châm chích, mẩn khi sản phẩm không hợp', 'RESULT_DA_DAU_NHAY_CAM', 5),
('Q3B_2', 'Không, da dầu nhưng ít khi kích ứng, chỉ nổi mụn nếu bít tắc', 'RESULT_DA_DAU', 5),

-- Options for Q2C (Da Hỗn Hợp)
('Q2C_1', 'Má khô hoặc hơi bong, trong khi vùng T thì dầu', 'Q3C', 6),
('Q2C_2', 'Má vẫn ẩm mượt hoặc bình thường, vùng T chỉ hơi bóng dầu', 'Q3C', 6),

-- Options for Q3C
('Q3C_1', 'Có, mình dễ bị ửng đỏ, có vẻ hơi nhạy cảm', 'RESULT_DA_HON_HOP_NHAY_CAM', 7),
('Q3C_2', 'Không, chỉ bị dầu ở T-zone và đôi khi khô nhẹ ở má', 'RESULT_DA_HON_HOP', 7),

-- Options for Q2D (Da Thường)
('Q2D_1', 'Hiếm lắm, đa số da vẫn ổn định', 'Q3D', 8),
('Q2D_2', 'Thỉnh thoảng có bị, nhưng thường do stress hoặc thay đổi nội tiết', 'Q3D', 8),

-- Options for Q3D
('Q3D_1', 'Vẫn mịn màng, ít bị khô căng', 'RESULT_DA_THUONG', 9),
('Q3D_2', 'Có hơi khô hoặc đổ dầu nhẹ, nhưng không quá đáng kể', 'RESULT_DA_THUONG', 9),

-- Options for Q2E (Da Nhạy Cảm)
('Q2E_1', 'Có, khá thường xuyên, mình sợ đổi mỹ phẩm mới', 'Q3E', 10),
('Q2E_2', 'Chỉ khi sản phẩm đó có thành phần tẩy rửa mạnh, còn loại nhẹ thì ổn', 'Q3E', 10),

-- Options for Q3E
('Q3E_1', 'Thiên về khô, sờ vào hay cảm giác căng', 'RESULT_DA_KHO_NHAY_CAM', 11),
('Q3E_2', 'Thiên về dầu, thường đổ dầu nhiều', 'RESULT_DA_DAU_NHAY_CAM', 11),
('Q3E_3', 'Không quá khô hay dầu, có lẽ nhạy cảm + hỗn hợp hoặc thường', 'RESULT_DA_THUONG_NHAY_CAM', 11),

-- Options for Result nodes (END option for each result)
('RESULT_DA_KHO_END', 'Hoàn thành', 'END', 12),
('RESULT_DA_KHO_NHAY_CAM_END', 'Hoàn thành', 'END', 13),
('RESULT_DA_DAU_END', 'Hoàn thành', 'END', 14),
('RESULT_DA_DAU_NHAY_CAM_END', 'Hoàn thành', 'END', 15),
('RESULT_DA_HON_HOP_END', 'Hoàn thành', 'END', 16),
('RESULT_DA_HON_HOP_NHAY_CAM_END', 'Hoàn thành', 'END', 17),
('RESULT_DA_THUONG_END', 'Hoàn thành', 'END', 18),
('RESULT_DA_THUONG_NHAY_CAM_END', 'Hoàn thành', 'END', 19);

-- Create placeholders for required non-NULL values
DECLARE @PlaceholderNextNodeId VARCHAR(50) = 'PLACEHOLDER_NEXT';
DECLARE @RootNodeId VARCHAR(50) = 'ROOT_NODE';
DECLARE @DefaultAttributes NVARCHAR(MAX) = '{"type":"default","description":"Default attributes"}';

-- Create the root node with non-NULL values for all required columns
INSERT INTO [Nodes] (Id, Content, ParentNodeId, NextNodeId, SurveyId, Attributes) 
VALUES (@RootNodeId, 'Root Node', @RootNodeId, @PlaceholderNextNodeId, 1, @DefaultAttributes);

-- Insert all Nodes with required non-NULL values
INSERT INTO [Nodes] (Id, Content, ParentNodeId, NextNodeId, SurveyId, Attributes) VALUES
-- Main flow nodes
('N_Q1', 'Initial question', @RootNodeId, @PlaceholderNextNodeId, 1, @DefaultAttributes),

-- Da Khô branch
('N_Q2A', 'Da Khô - Question 2', 'N_Q1', @PlaceholderNextNodeId, 2, @DefaultAttributes),
('N_Q3A', 'Da Khô - Question 3', 'N_Q2A', @PlaceholderNextNodeId, 3, @DefaultAttributes),
('N_RESULT_DA_KHO', 'Result: Da Khô', 'N_Q3A', @PlaceholderNextNodeId, 12, @DefaultAttributes),
('N_RESULT_DA_KHO_NHAY_CAM', 'Result: Da Khô + Nhạy Cảm', 'N_Q3A', @PlaceholderNextNodeId, 13, @DefaultAttributes),

-- Da Dầu branch
('N_Q2B', 'Da Dầu - Question 2', 'N_Q1', @PlaceholderNextNodeId, 4, @DefaultAttributes),
('N_Q3B', 'Da Dầu - Question 3', 'N_Q2B', @PlaceholderNextNodeId, 5, @DefaultAttributes),
('N_RESULT_DA_DAU', 'Result: Da Dầu', 'N_Q3B', @PlaceholderNextNodeId, 14, @DefaultAttributes),
('N_RESULT_DA_DAU_NHAY_CAM', 'Result: Da Dầu + Nhạy Cảm', 'N_Q3B', @PlaceholderNextNodeId, 15, @DefaultAttributes),

-- Da Hỗn Hợp branch
('N_Q2C', 'Da Hỗn Hợp - Question 2', 'N_Q1', @PlaceholderNextNodeId, 6, @DefaultAttributes),
('N_Q3C', 'Da Hỗn Hợp - Question 3', 'N_Q2C', @PlaceholderNextNodeId, 7, @DefaultAttributes),
('N_RESULT_DA_HON_HOP', 'Result: Da Hỗn Hợp', 'N_Q3C', @PlaceholderNextNodeId, 16, @DefaultAttributes),
('N_RESULT_DA_HON_HOP_NHAY_CAM', 'Result: Da Hỗn Hợp + Nhạy Cảm', 'N_Q3C', @PlaceholderNextNodeId, 17, @DefaultAttributes),

-- Da Thường branch
('N_Q2D', 'Da Thường - Question 2', 'N_Q1', @PlaceholderNextNodeId, 8, @DefaultAttributes),
('N_Q3D', 'Da Thường - Question 3', 'N_Q2D', @PlaceholderNextNodeId, 9, @DefaultAttributes),
('N_RESULT_DA_THUONG', 'Result: Da Thường', 'N_Q3D', @PlaceholderNextNodeId, 18, @DefaultAttributes),

-- Da Nhạy Cảm branch
('N_Q2E', 'Da Nhạy Cảm - Question 2', 'N_Q1', @PlaceholderNextNodeId, 10, @DefaultAttributes),
('N_Q3E', 'Da Nhạy Cảm - Question 3', 'N_Q2E', @PlaceholderNextNodeId, 11, @DefaultAttributes),
('N_RESULT_DA_THUONG_NHAY_CAM', 'Result: Da Thường + Nhạy Cảm', 'N_Q3E', @PlaceholderNextNodeId, 19, @DefaultAttributes);

-- Add special nodes with explicit attributes
INSERT INTO [Nodes] (Id, Content, ParentNodeId, NextNodeId, SurveyId, Attributes) VALUES
('N_SKIN_TEST_START', 'Start of Skin Test', @RootNodeId, 'N_Q1', 1, '{"type":"start","description":"Begin the skin type assessment test"}'),
('N_SKIN_TEST_END', 'End of Skin Test', @RootNodeId, @PlaceholderNextNodeId, 1, '{"type":"end","description":"End of the skin type assessment test"}');

-- Update Next Node IDs to create proper flow
UPDATE [Nodes] SET NextNodeId = 'N_Q2A' WHERE Id = 'N_Q1';
UPDATE [Nodes] SET NextNodeId = 'N_Q3A' WHERE Id = 'N_Q2A';
UPDATE [Nodes] SET NextNodeId = 'N_RESULT_DA_KHO_NHAY_CAM' WHERE Id = 'N_Q3A';
UPDATE [Nodes] SET NextNodeId = 'N_SKIN_TEST_END' WHERE Id IN ('N_RESULT_DA_KHO', 'N_RESULT_DA_KHO_NHAY_CAM', 'N_RESULT_DA_DAU', 'N_RESULT_DA_DAU_NHAY_CAM', 'N_RESULT_DA_HON_HOP', 'N_RESULT_DA_HON_HOP_NHAY_CAM', 'N_RESULT_DA_THUONG', 'N_RESULT_DA_THUONG_NHAY_CAM');