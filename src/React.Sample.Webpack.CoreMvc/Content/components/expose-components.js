import React from 'react';
import ReactDOM from 'react-dom';
import ReactDOMServer from 'react-dom/server';

import RootComponent from './home.jsx';
import { ServerStyleSheet } from 'styled-components';
import { JssProvider, SheetsRegistry } from 'react-jss';
import { renderStylesToString } from 'emotion-server';
import Helmet from 'react-helmet';

global.React = React;
global.ReactDOM = ReactDOM;
global.ReactDOMServer = ReactDOMServer;

global.Styled = { ServerStyleSheet };
global.ReactJss = { JssProvider, SheetsRegistry };
global.EmotionServer = { renderStylesToString };
global.Helmet = Helmet;

global.RootComponent = RootComponent;
