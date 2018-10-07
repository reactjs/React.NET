import React from 'react';
import injectSheet from 'react-jss';

const styles = {
    demoTitle: {
        color: 'lightslategray',
        fontFamily: 'Helvetica, sans-serif',
        textShadow: '0 0 5px lightblue',
    }
};

const DemoTitle = ({ classes, children }) => (
    <h1 className={classes.demoTitle}>
        Hello from React-JSS!
    </h1>
);

const WithInjectedSheet = injectSheet(styles)(DemoTitle);

export class ReactJssDemo extends React.Component {
    componentDidMount() {
        const serverStyles = document.getElementById('server-side-styles');
        if (serverStyles) {
            serverStyles.parentNode.removeChild(serverStyles);
        }
    }

    render() {
        return <WithInjectedSheet />;
    }
}
