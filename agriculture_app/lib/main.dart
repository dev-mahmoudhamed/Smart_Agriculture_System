import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:http/http.dart' as http;
import 'package:http_parser/http_parser.dart';
import 'dart:io';
import 'dart:typed_data';
import 'package:flutter/foundation.dart' show kIsWeb;
import 'dart:convert';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      home: ImageAndTextUploadScreen(),
    );
  }
}

class ImageAndTextUploadScreen extends StatefulWidget {
  const ImageAndTextUploadScreen({super.key});

  @override
  _ImageAndTextUploadScreenState createState() =>
      _ImageAndTextUploadScreenState();
}

class _ImageAndTextUploadScreenState extends State<ImageAndTextUploadScreen> {
  XFile? _image;
  Uint8List? _webImage;
  final ImagePicker _picker = ImagePicker();
  final TextEditingController _textController = TextEditingController();
  bool _isLoading = false;
  String? _response;

  // دالة اختيار الصورة
  Future<void> _pickImage() async {
    final pickedFile = await _picker.pickImage(source: ImageSource.gallery);
    if (pickedFile != null) {
      if (kIsWeb) {
        var bytes = await pickedFile.readAsBytes();
        setState(() {
          _webImage = bytes;
        });
      } else {
        setState(() {
          _image = pickedFile;
        });
      }
    }
  }

  // دالة إرسال الصورة والنص للخادم
  Future<void> _sendData() async {
    if (_image == null && _webImage == null) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text("Please pick an image first.")));
      return;
    }

    String textPrompt = _textController.text;

    setState(() {
      _isLoading = true;
    });

    // بناء رابط الطلب مع النص
    String apiUrl =
        "http://localhost:5005/api/gemini/askAI?TextPrompt=${Uri.encodeComponent(textPrompt)}";

    try {
      var request = http.MultipartRequest('POST', Uri.parse(apiUrl));
      request.headers['accept'] = '*/*';
      request.headers['Content-Type'] = 'multipart/form-data';

      if (kIsWeb) {
        request.files.add(
          http.MultipartFile.fromBytes(
            'ImageFile',
            _webImage!,
            filename: "image.jpeg",
            contentType: MediaType("image", "jpeg"),
          ),
        );
      } else {
        request.files.add(
          await http.MultipartFile.fromPath(
            'ImageFile',
            _image!.path,
            contentType: MediaType("image", "png"),
          ),
        );
      }

      var streamedResponse = await request.send();
      var response = await http.Response.fromStream(streamedResponse);

      setState(() {
        _isLoading = false;
        if (response.statusCode == 200) {
          var jsonResponse = jsonDecode(response.body);
          _response =
              jsonResponse['candidates'][0]['content']['parts'][0]['text'];
        } else {
          _response = "Error: ${response.body}";
        }
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
        _response = "Failed to connect to the API.";
      });
    }
  }

  // دالة عرض الصورة
  Widget _buildImageWidget() {
    if (kIsWeb) {
      return _webImage != null
          ? Image.memory(_webImage!, height: 150)
          : _placeholder();
    } else {
      return _image != null
          ? Image.file(File(_image!.path), height: 150)
          : _placeholder();
    }
  }

  Widget _placeholder() {
    return Container(
      height: 150,
      color: Colors.grey[50],
      child: Icon(Icons.image, size: 150, color: Colors.grey),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text("AI Assistant"), centerTitle: true),
      body: Padding(
        padding: const EdgeInsets.all(13.0),
        child: Column(
          children: [
            _buildImageWidget(),
            SizedBox(height: 20),
            ElevatedButton(onPressed: _pickImage, child: Text("Pick Image")),
            SizedBox(height: 20),
            TextField(
              controller: _textController,
              decoration: InputDecoration(
                labelText: "Enter TextPrompt",
                border: OutlineInputBorder(),
              ),
            ),
            SizedBox(height: 20),
            ElevatedButton(
              onPressed: _sendData,
              child:
                  _isLoading
                      ? CircularProgressIndicator(color: Colors.white)
                      : Text("Ask"),
            ),
            SizedBox(height: 20),
            if (_response != null)
              Expanded(child: SingleChildScrollView(child: Text(_response!))),
          ],
        ),
      ),
    );
  }
}
