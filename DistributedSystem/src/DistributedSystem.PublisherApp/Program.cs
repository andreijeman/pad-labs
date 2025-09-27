        using Terminal.Gui;

        Application.Init();

        var top = Application.Top;

        var win = new Window("Console UI Demo")
        {
            X = 0,
            Y = 1, // Leave space for menu bar
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        // Connect button
        var connectButton = new Button("Connect")
        {
            X = 1,
            Y = 1,
        };

        // System output box (read-only text view)
        var systemOutput = new TextView()
        {
            X = 1,
            Y = Pos.Bottom(connectButton) + 1,
            Width = Dim.Fill() - 2,
            Height = 5,
            ReadOnly = true,
            WordWrap = true,
        };

        // Input label and text field
        var inputLabel = new Label("Input:")
        {
            X = 1,
            Y = Pos.Bottom(systemOutput) + 1
        };

        var inputField = new TextField("")
        {
            X = Pos.Right(inputLabel) + 1,
            Y = Pos.Bottom(systemOutput) + 1,
            Width = 40
        };

        // Text box where input will be placed
        var inputEcho = new TextView()
        {
            X = 1,
            Y = Pos.Bottom(inputField) + 2,
            Width = Dim.Fill() - 2,
            Height = 5,
            ReadOnly = true,
            WordWrap = true
        };

        // Button click event → open connection dialog
        connectButton.Clicked += () =>
        {
            var dialog = new Dialog("Connect to Server", 60, 15);

            var ipLabel = new Label("IP:")
            {
                X = 2,
                Y = 2
            };
            var ipField = new TextField("127.0.0.1")
            {
                X = Pos.Right(ipLabel) + 1,
                Y = 2,
                Width = 30
            };

            var portLabel = new Label("Port:")
            {
                X = 2,
                Y = 4
            };
            var portField = new TextField("8080")
            {
                X = Pos.Right(portLabel) + 1,
                Y = 4,
                Width = 10
            };

            var okButton = new Button("OK")
            {
                X = Pos.Center(),
                Y = 8,
                IsDefault = true
            };

            okButton.Clicked += () =>
            {
                string ip = ipField.Text.ToString();
                string port = portField.Text.ToString();
                systemOutput.Text += $"Trying to connect to {ip}:{port}...\n";
                Application.RequestStop(); // close dialog
            };

            dialog.Add(ipLabel, ipField, portLabel, portField, okButton);
            Application.Run(dialog);
        };

        // Handle Enter key in input box
        inputField.KeyPress += (args) =>
        {
            if (args.KeyEvent.Key == Key.Enter)
            {
                var text = inputField.Text.ToString();
                inputEcho.Text += text + "\n";
                inputField.Text = ""; // clear after submit
                args.Handled = true;
            }
        };

        var quitButton = new Button("Quit")
        {
            X = Pos.Right(connectButton) + 2,
            Y = 1
        };

        quitButton.Clicked += () => Application.RequestStop();
        
        win.Add(quitButton, connectButton, systemOutput, inputLabel, inputField, inputEcho);
        top.Add(win);

        Application.Run();